using System;
using System.Collections.Generic;
using System.Text;

namespace HeadlessChicken.Core.Crawling
{
    public enum LinkEnqueueType
    {
        None = 0,
        SameDomain = 1,
        DifferentDomain = 2,
        URIMatchesRegex = 4,
        ExistsInWhitelist = 8,
        All = 16
    }
}
