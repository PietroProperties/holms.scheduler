using HOLMS.Scheduler.Jobs;
using Quartz;
using Microsoft.Extensions.Logging;

namespace HOLMS.Scheduler.Schedulers {
    class DailyOpExEmailScheduler : TaskSchedulerBase {
        public DailyOpExEmailScheduler(ILogger logger, ISchedulerFactory sf) : 
            base(logger, sf) { }
        
        public override void Schedule() {
            var sched = SF.GetScheduler();
            var basedata = new JobDataMap();

            var job = JobBuilder
                .Create<DailyOpExEmailJob>()
                .WithIdentity(DailyOpExEmailJob.JobNameString,
                    DailyOpExEmailJob.JobGroupString)
                .SetJobData(basedata)
                .Build();
            Logger.LogInformation($"Scheduling operational exception summary email job to run once per {DailyOpExEmailJob.JobPeriodHours} hours");
            var trigger = TriggerBuilder.Create()
                .WithIdentity("DailyOpExEmailPeriodic", DailyOpExEmailJob.JobGroupString)
                .ForJob(job.Key)
                .StartAt(DateBuilder.FutureDate(30, IntervalUnit.Second))
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(DailyOpExEmailJob.JobPeriodHours)
                    .RepeatForever())
                .Build();
            sched.ScheduleJob(job, trigger);
        }
    }
}
