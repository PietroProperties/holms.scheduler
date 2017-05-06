using HOLMS.Scheduler.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;
namespace HOLMS.Scheduler.Schedulers {
    class CardAuthorizationCleanupScheduler : TaskSchedulerBase {
        public CardAuthorizationCleanupScheduler(ILogger logger, ISchedulerFactory sf)
            : base(logger, sf) { }

        public override void Schedule() {
            var sched = SF.GetScheduler();
            var basedata = new JobDataMap();

            var job = JobBuilder
                .Create<AuthorizationCleanupJob>()
                .WithIdentity(AuthorizationCleanupJob.JobNameString, AuthorizationCleanupJob.JobGroupString)
                .SetJobData(basedata)
                .Build();

            Logger.LogInformation("Scheduling authorization cleanup job to run nightly at 2:30AM");

            var trigger = TriggerBuilder.Create()
                .WithIdentity("AuthorizationCleanupNightly", AuthorizationCleanupJob.JobGroupString)
                .ForJob(AuthorizationCleanupJob.JobNameString, AuthorizationCleanupJob.JobGroupString)
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(2, 30))
                .Build();


            sched.ScheduleJob(job, trigger);
        }
    }
}
