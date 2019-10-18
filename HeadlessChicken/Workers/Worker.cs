using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HeadlessChicken.Abstractions;
using HeadlessChicken.Core.Enums;
using HeadlessChicken.Core.Models;
using HeadlessChicken.Core.Pausing;
using HeadlessChicken.Core.Progress;
using HeadlessChicken.Models;
using PuppeteerSharp;

namespace HeadlessChicken.Workers
{
    internal class Worker : IWorker
    {
        public int ID { get; }
        public bool Running { get; private set; }
        public bool HasWork { get; private set; }
        public ManualResetEventSlim DoneEvent { get; private set; }
        public Thread Thread { get; private set; }

        public Worker(int id)
        {
            ID = id;
            DoneEvent = new ManualResetEventSlim(false);
        }

        public void Start(Browser browser,
            CancellationToken cancellationToken,
            PauseToken pauseToken,
            WorkerRelevantJobData jobData,
            ConcurrentQueue<Uri> queue,
            ConcurrentDictionary<Uri, CrawlData> crawled)
        {
            Thread = new Thread(
                async() => await ThreadWork(
                    browser, 
                    cancellationToken, 
                    pauseToken, 
                    jobData, 
                    queue, 
                    crawled));

            Thread.Start();
        }

        private async Task ThreadWork(
            Browser browser,
            CancellationToken cancellationToken, 
            PauseToken pauseToken,
            WorkerRelevantJobData jobData,
            ConcurrentQueue<Uri> queue,
            ConcurrentDictionary<Uri, CrawlData> crawled)
        {
            try
            {
                Running = true;
                var tab = await browser.NewPageAsync();

                // create a new tab on the browser for this thread

                while (!cancellationToken.IsCancellationRequested)
                {
                    // get something from the queue
                    // if nothing wait, set haswork false, try again
                    // if something, set haswork and work
                    var nextUri = DequeueOrRetry(queue, cancellationToken);

                    // nextUri could be null if the cancellation was requested while in above method
                    // this just short circuits before any resources are wasted or null ref exceptions occur
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    // if pause token, spin here
                    if (pauseToken.IsPaused)
                    {
                        if (PauseHolder(cancellationToken, pauseToken))
                        {
                            break;
                        }
                    }

                    // go to page
                    var response = await tab.GoToAsync(nextUri.AbsolutePath);
                    if (response.Ok)
                    {
                        // perform the jobs actions
                        // each action could yield a return value, such as extracted data
                        // the url should be added to the crawl collection 
                    }
                    else
                    {
                        // indicate in the crawled collection this was a failure + reason
                        crawled.TryAdd(nextUri, CrawlData.CreateFailureData(response.Status, response.StatusText));
                    }

                    // if we should look for some links on the page
                    if (jobData.LinkEnqueueType != LinkEnqueueType.None)
                    {
                        // get the page content and just put it in a collection
                        // parser group will sort through and add the links
                    }
                }
            }
            catch (ThreadAbortException e)
            {

            }
            finally
            {
                Running = false;
                DoneEvent.Set();
            }
        }

        private Uri DequeueOrRetry(ConcurrentQueue<Uri> queue, CancellationToken cancellationToken)
        {
            HasWork = false;
            Uri nextUri = null;
            while (!queue.TryDequeue(out nextUri) && !cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(10);
            }

            // cancellation token cancelling also brings us here, so it's not a guarantee it has work, just passing through
            HasWork = nextUri != null;
            return nextUri;
        }

        /// <summary>
        /// Holds the thread while the pause token is paused, if cancellation is detected this will return true (false otherwise)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="pauseToken"></param>
        /// <returns></returns>
        private bool PauseHolder(CancellationToken cancellationToken, PauseToken pauseToken)
        {
            while (pauseToken.IsPaused)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return true;
                }
                Thread.Sleep(10);
            }

            return false;
        }
    }
}
