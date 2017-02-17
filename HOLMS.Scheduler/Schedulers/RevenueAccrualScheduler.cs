using System;
using System.Linq;
using HOLMS.Application.Client;
using HOLMS.Scheduler.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    class RevenueAccrualScheduler : TaskSchedulerBase {
        private const string ImmediateRunFlag = "--immediateaccrual";
        private bool _immediateRunRequested;

        public RevenueAccrualScheduler(ILogger logger, ISchedulerFactory sf)
            : base(logger, sf) { }

        public override void ParseCommandLineArgs(string[] args) {
            _immediateRunRequested = args.Contains(ImmediateRunFlag);
        }

        public override void Schedule() {
            var sched = SF.GetScheduler();
            var basedata = new JobDataMap();

            var job = JobBuilder
                .Create<RevenueAccrualJob>()
                .WithIdentity(RevenueAccrualJob.JobNameString, RevenueAccrualJob.JobGroupString)
                .SetJobData(basedata)
                .Build();
            
            if (_immediateRunRequested) {
                Logger.LogInformation($"Scheduling immediate run of revenue accrual job (one run only, Vasili)");

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("RevenueAccrualImmediate", RevenueAccrualJob.JobGroupString)
                    .ForJob(RevenueAccrualJob.JobNameString, RevenueAccrualJob.JobGroupString)
                    .WithSimpleSchedule(x => x.WithInterval(new TimeSpan(0, 0, 0)).WithRepeatCount(0))
                    .Build();

                sched.ScheduleJob(job, trigger);
            } else {
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
}
