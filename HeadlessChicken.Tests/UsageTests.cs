using System.Threading;
using System.Threading.Tasks;
using HeadlessChicken.Core.Models;
using HeadlessChicken.Core.Pausing;
using HeadlessChicken.Core.Progress;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeadlessChicken.Tests
{
    [TestClass]
    public class UsageTests
    {
        [TestMethod]
        public async Task Usage1()
        {
            using (var crawler = new Crawler())
            {
                var job = new JobDTO
                {

                };

                var cancellationTokenSource = new CancellationTokenSource();
                var pauseTokenSource = new PauseTokenSource();
                var progressToken = new ProgressToken();

                var crawlTask = crawler.Start(job, cancellationTokenSource.Token, pauseTokenSource.Token, progressToken);
                
                while (!crawlTask.IsCompleted && !crawlTask.IsCanceled)
                {

                }

                var result = await crawlTask;

                
            }
        }
    }
}
