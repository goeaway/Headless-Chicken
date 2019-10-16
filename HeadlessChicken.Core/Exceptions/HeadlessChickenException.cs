using System;
using System.Collections.Generic;
using System.Text;

namespace HeadlessChicken.Core.Exceptions
{
    public class HeadlessChickenException : Exception
    {
        public HeadlessChickenException() { }
        public HeadlessChickenException(string message) : base(message) { }
        public HeadlessChickenException(string message, Exception innerException) : base(message, innerException) { }
    }
}
