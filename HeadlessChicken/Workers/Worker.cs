﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HeadlessChicken.Abstractions;
using HeadlessChicken.Core.Pausing;
using HeadlessChicken.Core.Progress;
using HeadlessChicken.Models;
using PuppeteerSharp;

namespace HeadlessChicken.Workers
{
    internal class Worker : IWorker
    {
        public int ID { get; }
        public bool Running { get; private set; }
        public bool HasWork { get; private set; }
        public ManualResetEvent DoneEvent { get; private set; }
        public Thread Thread { get; private set; }

        public Worker(int id, ManualResetEvent doneEvent)
        {
            ID = id;
            DoneEvent = doneEvent;
        }

        public void StartThread(Browser browser,
            CancellationToken cancellationToken,
            PauseToken pauseToken,
            WorkerRelevantJobData jobData,
            ConcurrentQueue<Uri> queue,
            ConcurrentDictionary<Uri, string> crawled)
        {
            Thread = new Thread(
                async() => await ThreadWork(
                    browser, 
                    cancellationToken, 
                    pauseToken, 
                    jobData, 
                    queue, 
                    crawled));

            Thread.Start();
        }

        private async Task ThreadWork(
            Browser browser,
            CancellationToken cancellationToken, 
            PauseToken pauseToken,
            WorkerRelevantJobData jobData,
            ConcurrentQueue<Uri> queue,
            ConcurrentDictionary<Uri, string> crawled)
        {
            try
            {
                Running = true;

                // create a new tab on the browser for this thread

                while (!cancellationToken.IsCancellationRequested)
                {
                    // get something from the queue
                    // if nothing wait, set haswork false, try again
                    // if something, set haswork and work
                    var nextUri = DequeueOrRetry(queue, cancellationToken);

                    // nextUri could be null if the cancellation was requested while in above method
                    // this just short circuits before any resources are wasted or null ref exceptions occur
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

                    await browser.GetUserAgentAsync();
                    // put result in crawl dict

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

        private Uri DequeueOrRetry(ConcurrentQueue<Uri> queue, CancellationToken cancellationToken)
        {
            HasWork = false;
            Uri nextUri = null;
            while (!queue.TryDequeue(out nextUri) && !cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(10);
            }

            // cancellation token cancelling also brings us here, so it's not a guarantee it has work, just passing through
            HasWork = nextUri != null;
            return nextUri;
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
    }
}