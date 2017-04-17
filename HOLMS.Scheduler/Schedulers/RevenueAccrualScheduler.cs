using HOLMS.Scheduler.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    class RevenueAccrualScheduler : TaskSchedulerBase {

        public RevenueAccrualScheduler(ILogger logger, ISchedulerFactory sf)
            : base(logger, sf) { }

        public override void Schedule() {
            var sched = SF.GetScheduler();
            var basedata = new JobDataMap();

            var job = JobBuilder
                .Create<RevenueAccrualJob>()
                .WithIdentity(RevenueAccrualJob.JobNameString, RevenueAccrualJob.JobGroupString)
                .SetJobData(basedata)
                .Build();
            Logger.LogInformation($"Scheduling revenue accrual job to run once per {RevenueAccrualJob.JobPeriodMins} minutes");    

            var trigger = TriggerBuilder.Create()
                .WithIdentity("RevenueAccrualPeriodic", RevenueAccrualJob.JobGroupString)
                .ForJob(RevenueAccrualJob.JobNameString, RevenueAccrualJob.JobGroupString)
                .WithSimpleSchedule(x => x
                    .WithIntervalInMinutes(RevenueAccrualJob.JobPeriodMins)
                    .RepeatForever())
                .Build();

                
            sched.ScheduleJob(job, trigger);
        }
    }
}
