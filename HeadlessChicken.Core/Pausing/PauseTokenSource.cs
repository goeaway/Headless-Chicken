using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HeadlessChicken.Core.Pausing
{
    public class PauseTokenSource
    {
        private volatile TaskCompletionSource<bool> paused;

        public bool IsPaused
        {
            get => paused != null;
            set
            {
                if (value)
                {
                    Interlocked.CompareExchange(
                        ref paused, new TaskCompletionSource<bool>(), null);
                }
                else
                {
                    while (true)
                    {
                        var tcs = paused;
                        if (tcs == null) return;
                        if (Interlocked.CompareExchange(ref paused, null, tcs) == tcs)
                        {
                            tcs.SetResult(true);
                            break;
                        }
                    }
                }
            }
        }

        public PauseToken Token => new PauseToken(this);
    }
}
