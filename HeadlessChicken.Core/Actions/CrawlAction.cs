using System;
using System.Collections.Generic;
using System.Text;
using HeadlessChicken.Core.Enums;

namespace HeadlessChicken.Core.Actions
{
    public class CrawlAction
    {
        public ElementSelector Selector { get; set; }
        public TimeSpan SelectorSearchTimeout { get; set; }
        public string ExtractDataName { get; set; }
        public TimeSpan WaitPeriod { get; set; }
        public CrawlActionType ActionType { get; set; }
    }
}
