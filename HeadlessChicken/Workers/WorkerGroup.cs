﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HeadlessChicken.Core.Models;
using HeadlessChicken.Core.Pausing;
using HeadlessChicken.Core.Progress;
using HeadlessChicken.Exceptions;
using HeadlessChicken.Models;
using PuppeteerSharp;

namespace HeadlessChicken.Workers
{
    internal class WorkerGroup
    {
        private readonly Browser _browser;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ICollection<Worker> _workers;

        public bool AllDone => _workers.All(w => !w.DoneEvent.WaitOne(1));
        public bool SomeDone => _workers.Any(w => w.DoneEvent.WaitOne(1));

        public WorkerGroup(
            Browser browser,
            int amount)
        {
            if (amount < 1 || amount > 64)
            {
                throw new WorkerGroupThreadCountOutOfBounds(amount);
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _workers = new List<Worker>(amount);

            for (int i = 0; i < amount; i++)
            {
                var doneEvent = new ManualResetEvent(false);
                _workers.Add(
                    new Worker(i, doneEvent)
                );
            }
        }

        public void StartWorkers(
            PauseToken pauseToken,
            WorkerRelevantJobData jobData,
            ConcurrentQueue<Uri> queue,
            ConcurrentDictionary<Uri, string> crawled)
        {
            foreach (var worker in _workers)
            {
                worker.StartThread(
                    _browser, 
                    _cancellationTokenSource.Token, 
                    pauseToken, 
                    jobData, 
                    queue, 
                    crawled);
            }
        }

        public void DiposeWorkers()
        {
            foreach (var worker in _workers)
            {
                if (worker.Thread.ThreadState == ThreadState.Running)
                {
                    worker.Thread.Join();
                }
            }
        }

        public void AbortWorkers()
        {
            foreach (var worker in _workers)
            {
                // if the worker is not yet done, we abort
                if (!worker.DoneEvent.WaitOne(1))
                {
                    worker.Thread.Abort();
                }
            }
        }

        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }
    }
}