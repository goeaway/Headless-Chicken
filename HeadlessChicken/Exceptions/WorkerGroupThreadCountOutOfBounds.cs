using System;
using System.Collections.Generic;
using System.Text;
using HeadlessChicken.Core.Exceptions;

namespace HeadlessChicken.Exceptions
{
    public class WorkerGroupThreadCountOutOfBounds : HeadlessChickenException
    {
        public WorkerGroupThreadCountOutOfBounds(int amount) : base($"Attempted to set worker group thread amount to {amount}. Thread amount must be between 1 and 64") { }
    }
}
