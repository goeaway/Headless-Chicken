using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HeadlessChicken.Core.Models
{
    public class CrawlData : Dictionary<string, object>
    {
        public static CrawlData CreateFailureData(HttpStatusCode statusCode, string statusText)
            => new CrawlData
            {
                { "Code", statusCode },
                { "Error message", statusText }
            };
    }
}
