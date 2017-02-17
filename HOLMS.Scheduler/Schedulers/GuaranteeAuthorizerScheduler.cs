using System;
using System.Linq;
using HOLMS.Application.Client;
using HOLMS.Scheduler.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    class GuaranteeAuthorizerScheduler : TaskSchedulerBase {
        private const string ImmediateRunFlag = "--immediateguaranteeauthorization";
        private bool _immediateRunRequested;

        public GuaranteeAuthorizerScheduler(ILogger logger, ISchedulerFactory sf)
            : base(logger, sf) { }

        public override void ParseCommandLineArgs(string[] args) {
            _immediateRunRequested = args.Contains(ImmediateRunFlag);
        }

        public override void Schedule() {
            var sched = SF.GetScheduler();
            var basedata = new JobDataMap();

            var job = JobBuilder
                .Create<RevenueAccrualJob>()
                .WithIdentity(GuaranteeAuthorizerJob.JobNameString, GuaranteeAuthorizerJob.JobGroupString)
                .SetJobData(basedata)
                .Build();

            if (_immediateRunRequested) {
                Logger.LogInformation($"Scheduling immediate run of guarantee authorizer");

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("GuaranteeAuthorizationImmediate", GuaranteeAuthorizerJob.JobGroupString)
                    .ForJob(GuaranteeAuthorizerJob.JobNameString, GuaranteeAuthorizerJob.JobGroupString)
                    .WithSimpleSchedule(x => x.WithInterval(new TimeSpan(0, 0, 0)).WithRepeatCount(0))
                    .Build();

                sched.ScheduleJob(job, trigger);
            } else {
                Logger.LogInformation($"Scheduling guarantee authorizer job to run once per {RevenueAccrualJob.JobPeriodMins} minutes");

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("GuaranteeAuthorizationNightly", GuaranteeAuthorizerJob.JobGroupString)
                    .ForJob(GuaranteeAuthorizerJob.JobNameString, GuaranteeAuthorizerJob.JobGroupString)
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(2, 0))
                    .Build();


                sched.ScheduleJob(job, trigger);
            }
        }
    }
}
