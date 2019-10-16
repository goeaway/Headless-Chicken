using System;
using System.Collections.Generic;
using System.Text;

namespace HeadlessChicken
{
    internal static class Consts
    {
        public static TimeSpan WorkerAbortTimeout = TimeSpan.FromSeconds(30);
        public const int WorkerGroupCount = 1;
        public const int WorkerPerGroupCount = 1;
    }
}
