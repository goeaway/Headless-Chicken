using System;
using System.Collections.Generic;
using System.Text;

namespace HeadlessChicken.Abstractions
{
    internal interface IWorker
    {
        int ID { get; }
        bool Running { get; }
        bool HasWork { get; }
    }
}
