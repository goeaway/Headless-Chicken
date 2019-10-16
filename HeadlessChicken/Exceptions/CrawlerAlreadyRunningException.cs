using HeadlessChicken.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HeadlessChicken.Exceptions
{
    public class CrawlerAlreadyRunningException : HeadlessChickenException
    {
        public CrawlerAlreadyRunningException() { }
        public CrawlerAlreadyRunningException(string message) : base(message) { }
    }
}
