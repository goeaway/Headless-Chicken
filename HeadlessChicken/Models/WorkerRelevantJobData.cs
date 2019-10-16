using HeadlessChicken.Core.Actions;
using HeadlessChicken.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using HeadlessChicken.Core.Models;

namespace HeadlessChicken.Models
{
    internal class WorkerRelevantJobData
    {
        public IEnumerable<Uri> Seeds { get; set; }
        public LinkEnqueueType LinkEnqueueType { get; set; }
        public string LinkEnqueueRegex { get; set; }
        public IEnumerable<Uri> LinkEnqueueCollection { get; set; }
        public IEnumerable<CrawlAction> CrawlActions { get; set; }

        internal static WorkerRelevantJobData FromJobDTO(JobDTO jobDTO)
        {
            return new WorkerRelevantJobData
            {
                Seeds = jobDTO.Seeds,
                LinkEnqueueType = jobDTO.LinkEnqueueType,
                CrawlActions = jobDTO.CrawlActions,
                LinkEnqueueCollection = jobDTO.LinkEnqueueCollection,
                LinkEnqueueRegex = jobDTO.LinkEnqueueRegex
            };
        }
    }
}
