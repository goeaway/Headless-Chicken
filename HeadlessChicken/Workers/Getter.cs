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
    internal class Getter : WorkerBase
    {
        private Page _tab;

        public Getter(int id) : base(id)
        {
        }

        public void Start(Browser browser,
            CancellationToken cancellationToken,
            PauseToken pauseToken,
            WorkerRelevantJobData jobData,
            ConcurrentQueue<Uri> queue,
            ConcurrentQueue<string> htmlQueue,
            ConcurrentDictionary<Uri, CrawlData> crawled)
        {
            Thread = new Thread(
                async() => await WorkAction(
                    browser, 
                    cancellationToken, 
                    pauseToken, 
                    jobData, 
                    queue, 
                    crawled));

            Thread.Start();
            _tab = browser.NewPageAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private async Task WorkAction(
            Browser browser,
            CancellationToken cancellationToken, 
            PauseToken pauseToken,
            WorkerRelevantJobData jobData,
            ConcurrentQueue<Uri> queue,
            ConcurrentDictionary<Uri, CrawlData> crawled)
        {

            // create a new tab on the browser for this thread

            var nextUri = DequeueOrRetry(queue, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            // go to page
            var response = await _tab.GoToAsync(nextUri.AbsolutePath);
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
    }
}
