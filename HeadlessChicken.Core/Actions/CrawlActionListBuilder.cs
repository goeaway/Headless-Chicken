using System;
using System.Collections.Generic;
using System.Text;

namespace HeadlessChicken.Core.Actions
{
    public class CrawlActionListBuilder
    {
        private readonly ICollection<CrawlAction> _crawlActions
            = new List<CrawlAction>();

        public CrawlActionListBuilder AddAction(CrawlAction action)
        {
            _crawlActions.Add(action);

            return this;
        }

        public CrawlActionListBuilder 

        public IEnumerable<CrawlAction> GetList()
        {
            return _crawlActions;
        }
    }
}
