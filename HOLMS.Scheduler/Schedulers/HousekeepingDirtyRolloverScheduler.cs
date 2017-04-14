using HOLMS.Scheduler.Jobs;
using Quartz;
using System.Linq;
using HOLMS.Application.Client;
using Microsoft.Extensions.Logging;

namespace HOLMS.Scheduler.Schedulers {
    class HousekeepingDirtyRolloverScheduler : TaskSchedulerBase {
        public HousekeepingDirtyRolloverScheduler(ILogger logger, ISchedulerFactory sf) : 
            base(logger, sf) { }


        public override void Schedule() {
            var sched = SF.GetScheduler();
            var basedata = new JobDataMap();

            var job = JobBuilder
                .Create<HousekeepingDirtyRolloverJob>()
                .WithIdentity(HousekeepingDirtyRolloverJob.JobNameString,
                    HousekeepingDirtyRolloverJob.JobGroupString)
                .SetJobData(basedata)
                .Build();
            Logger.LogInformation($"Scheduling housekeeping dirty rollover job to run once per {HousekeepingDirtyRolloverJob.JobPeriodMins} minutes");
            var trigger = TriggerBuilder.Create()
                .WithIdentity("HousekeepingRolloverPeriodic", RevenueAccrualJob.JobGroupString)
                .ForJob(job.Key)
                .StartAt(DateBuilder.FutureDate(30, IntervalUnit.Second))
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(HousekeepingDirtyRolloverJob.JobPeriodMins)
                    .RepeatForever())
                .Build();
            sched.ScheduleJob(job, trigger);
        }
    }
}
