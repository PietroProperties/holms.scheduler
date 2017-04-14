using HOLMS.Scheduler.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    class GuaranteeAuthorizerScheduler : TaskSchedulerBase {
        public GuaranteeAuthorizerScheduler(ILogger logger, ISchedulerFactory sf)
            : base(logger, sf) { }

        public override void Schedule() {
            var sched = SF.GetScheduler();
            var basedata = new JobDataMap();

            var job = JobBuilder
                .Create<GuaranteeAuthorizerJob>()
                .WithIdentity(GuaranteeAuthorizerJob.JobNameString, GuaranteeAuthorizerJob.JobGroupString)
                .SetJobData(basedata)
                .Build();

            Logger.LogInformation("Scheduling guarantee authorizer job to run nightly at 2AM");

            var trigger = TriggerBuilder.Create()
                .WithIdentity("GuaranteeAuthorizationNightly", GuaranteeAuthorizerJob.JobGroupString)
                .ForJob(GuaranteeAuthorizerJob.JobNameString, GuaranteeAuthorizerJob.JobGroupString)
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(2, 0))
                .Build();


            sched.ScheduleJob(job, trigger);
        }
    }
}
