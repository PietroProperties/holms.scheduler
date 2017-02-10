using HOLMS.Scheduler.Jobs;
using Quartz;
using System.Linq;
using HOLMS.Application.Client;
using Microsoft.Extensions.Logging;

namespace HOLMS.Scheduler.Schedulers {
    class HousekeepingDirtyRolloverScheduler : TaskSchedulerBase {
        private const string ImmediateRunFlag = "--immediatehkrollover";
        private bool _immediateRunRequested;
        public HousekeepingDirtyRolloverScheduler(IApplicationClient ac, ISchedulerFactory sf) : 
            base(ac, sf) { }

        public override void ParseCommandLineArgs(string[] args) {
            _immediateRunRequested = args.Contains(ImmediateRunFlag);
        }

        public override void Schedule() {
            var sched = SF.GetScheduler();
            var basedata = new JobDataMap() {
                { HousekeepingDirtyRolloverJob.ImmediateRunBoolean, _immediateRunRequested}
            };

            var job = JobBuilder
                .Create<HousekeepingDirtyRolloverJob>()
                .WithIdentity(HousekeepingDirtyRolloverJob.JobNameString,
                    HousekeepingDirtyRolloverJob.JobGroupString)
                .SetJobData(basedata)
                .Build();

            if (_immediateRunRequested) {
                Logger.LogInformation("Scheduling single immediate run of the housekeeping dirty rollover job");
                var trigger = TriggerBuilder.Create()
                    .WithIdentity("HousekeepingRolloverImmediate",
                        HousekeepingDirtyRolloverJob.JobGroupString)
                    .ForJob(job.Key)
                    .StartAt(DateBuilder.FutureDate(10, IntervalUnit.Second))
                    .Build();
                sched.ScheduleJob(job, trigger);
            } else {
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
}
