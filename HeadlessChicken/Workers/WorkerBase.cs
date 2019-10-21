using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HeadlessChicken.Abstractions;
using HeadlessChicken.Core.Pausing;

namespace HeadlessChicken.Workers
{
    internal abstract class WorkerBase : IWorker
    {
        public int ID { get; }
        public bool Running { get; protected set; }
        public bool HasWork { get; protected set; }

        public ManualResetEventSlim DoneEvent { get; private set; }
        protected Thread Thread { get; set; }

        public WorkerBase(int id)
        {
            ID = id;
            DoneEvent = new ManualResetEventSlim(false);
        }

        protected async Task WorkLoop(
            CancellationToken cancellationToken, 
            PauseToken pauseToken,
            Action action)
        {
            try
            {
                Running = true;

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    // if pause token, spin here
                    if (pauseToken.IsPaused)
                    {
                        if (PauseHolder(cancellationToken, pauseToken))
                        {
                            break;
                        }
                    }

                    action();
                }
            }
            catch (ThreadAbortException e)
            {

            }
            finally
            {
                Running = false;
                DoneEvent.Set();
            }
        }

        /// <summary>
        /// Holds the thread while the pause token is paused, if cancellation is detected this will return true (false otherwise)
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="pauseToken"></param>
        /// <returns></returns>
        private bool PauseHolder(CancellationToken cancellationToken, PauseToken pauseToken)
        {
            while (pauseToken.IsPaused)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return true;
                }
                Thread.Sleep(10);
            }

            return false;
        }

        protected abstract Task WorkAction();
    }
}
