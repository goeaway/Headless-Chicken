using System;
using System.Collections.Generic;
using System.Text;
using HeadlessChicken.Core.Actions;
using HeadlessChicken.Core.Crawling;

namespace HeadlessChicken.Core.Models
{
    public class JobDTO
    {
        public IEnumerable<Uri> Seeds { get; set; }
        public LinkEnqueueType LinkEnqueueType { get; set; }
        public string LinkEnqueueRegex { get; set; }
        public IEnumerable<Uri> LinkEnqueueCollection { get; set; }
        public int GetterThreads { get; set; }
        public int ParserThreads { get; set; }

        public TimeSpan MaxTime { get; set; }
        public int MaxPageCrawl { get; set; }

        public IEnumerable<CrawlAction> CrawlActions { get; set; }
    }
}
