using HeadlessChicken.Core;
using HeadlessChicken.Core.Pausing;
using HeadlessChicken.Core.Progress;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp;
using HeadlessChicken.Core.Models;
using HeadlessChicken.Exceptions;
using HeadlessChicken.Models;
using HeadlessChicken.Workers;

namespace HeadlessChicken
{
    public class Crawler : IDisposable
    {
        private readonly Browser _browser;

        public bool IsCrawling { get; private set; }

        public TimeSpan AbortWorkersTimeout { get; set; }
            = Consts.WorkerAbortTimeout;

        public int WorkerGroupsCount { get; set; }
            = Consts.WorkerGroupCount;

        public int WorkersPerGroup { get; set; }
            = Consts.WorkerPerGroupCount;

        public Crawler()
        {
            // initialise by downloading the browser if needed
            new BrowserFetcher()
                .DownloadAsync(BrowserFetcher.DefaultRevision)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
            // set the browser, we'll pass it to threads that need it when required
            _browser = Puppeteer
                .LaunchAsync(new LaunchOptions {Headless = true})
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        public void Dispose()
        {
            if (_browser != null)
            {
                _browser.Dispose();
            }
        }

        private ProgressResult GetResult(ConcurrentQueue<Uri> queue, ConcurrentDictionary<Uri, CrawlData> crawled) 
        {
            return null;
        }

        private ProgressState GetState(ConcurrentQueue<Uri> queue, ConcurrentDictionary<Uri, CrawlData> crawled)
        {
            return null;
        }

        public Task<ProgressResult> Start(
            JobDTO job, 
            CancellationToken cancellationToken, 
            PauseToken pauseToken, 
            ProgressToken progressToken)
        {
            if (IsCrawling)
            {
                throw new CrawlerAlreadyRunningException();
            }

            return Task.Run(() =>
            {
                var workerRelevantJobData = WorkerRelevantJobData.FromJobDTO(job);
                // create worker groups (max 64 threads, could give them each a browser, can give them each a set of domains to work on)
                var group = new WorkerGroup(
                    _browser, 
                    WorkersPerGroup);

                var uriQueue = new ConcurrentQueue<Uri>(job.Seeds);
                var htmlQueue = new ConcurrentQueue<string>();
                var crawled = new ConcurrentDictionary<Uri, CrawlData>();

                group.StartWorkers(
                    pauseToken,
                    workerRelevantJobData,
                    uriQueue,
                    crawled);

                IsCrawling = true;

                // main thread will spin here, checking for cancellation, or progress requests
                // quit if the queue is empty and no one seems to be adding to it?
                // don't want to just quit in case the queue is empty because there might be something about to be added
                // but it's just taking a long time
                while (!cancellationToken.IsCancellationRequested /* TODO add other stop conditions such as max crawl time */)
                {
                    // cancellation and pauses are passed down to worker groups
                    // progress is checked via the collections and then updated
                    if (progressToken.ProgressIsRequested)
                    {
                        progressToken.State = GetState(uriQueue, crawled);
                    }
                    Thread.Sleep(10);
                }

                // tell the worker group to stop
                group.Cancel();
                var start = DateTime.Now;

                // after cancellation is requested we fall into another loop, this has a time limit, so if not all worker groups are done
                while (!group.AllDone && (DateTime.Now - start) < AbortWorkersTimeout)
                {
                    // notify of which threads are holding us up
                    Thread.Sleep(10);
                }

                // attempt to properly join the threads
                if (group.AllDone)
                {
                    group.DiposeWorkers();
                }
                // abort them if not done
                else
                {
                    group.AbortWorkers();
                }

                IsCrawling = false;

                return GetResult(uriQueue, crawled);
            });
        }
    }
}
