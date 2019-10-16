using System;
using System.Collections.Generic;
using System.Text;

namespace HeadlessChicken.Core.Progress
{
    public struct ProgressToken
    {
        public bool ProgressIsRequested { get; private set; }

        private ProgressState _state;
        public ProgressState State
        {
            get => _state;
            set
            {
                _state = value;
                ProgressIsRequested = false;
            }
        }
    }
}
