using HeadlessChicken.Core;
using HeadlessChicken.Core.Pausing;
using HeadlessChicken.Core.Progress;
using System;
using System.Threading;
using System.Threading.Tasks;
using PuppeteerSharp;
using HeadlessChicken.Core.Models;

namespace HeadlessChicken
{
    public class Chicken : IDisposable
    {
        private readonly Browser _browser;

        public Chicken()
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
            _browser.Dispose();
        }

        public async Task<ProgressResult> Start(JobDTO job, CancellationToken cancellationToken, PauseToken pauseToken, ProgressToken progressToken)
        {
            return null;
        }
    }
}
