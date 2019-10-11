using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeadlessChicken.Tests
{
    [TestClass]
    public class UsageTests
    {
        [TestMethod]
        public async Task Usage1()
        {
            using (var chicken = new Chicken())
            {
                var crawlTask = chicken.Start();

                while (!crawlTask.IsCompleted && !crawlTask.IsCanceled)
                {

                }

                var result = await crawlTask;

                
            }
        }
    }
}
