using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HOLMS.Application.Client;
using Quartz;
using HOLMS.Scheduler.Jobs;
using Microsoft.Extensions.Logging;

namespace HOLMS.Scheduler.Schedulers {
    class OTASyncScheduler : TaskSchedulerBase {
        private const int _secondsInterval = 180;

        public OTASyncScheduler(ILogger log, ISchedulerFactory scheduler) : base(log, scheduler) {
        }

        public override void Schedule() {
            var sched = SF.GetScheduler();
            var basedata = new JobDataMap();

            var job = JobBuilder
                .Create<OTASyncJob>()
                .WithIdentity(OTASyncJob.JobNameString, OTASyncJob.JobGroupString)
                .SetJobData(basedata)
                .Build();
            Logger.LogInformation($"Scheduling ota synchronizer to run ever {_secondsInterval} seconds");

            var trigger = TriggerBuilder.Create()
                .WithIdentity("OTASynchronizationImmediate", OTASyncJob.JobGroupString)
                .ForJob(OTASyncJob.JobNameString, OTASyncJob.JobGroupString)
                .WithSimpleSchedule(x => x.WithInterval(new TimeSpan(0, 0, _secondsInterval)).RepeatForever())
                .Build();

            sched.ScheduleJob(job, trigger);
        }
    }
}
