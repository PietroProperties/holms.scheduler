using System;
using HOLMS.Scheduler.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    class RecurringJobScheduler<T> : TaskSchedulerBase where T : QuartzJobBase {
        private readonly string _jobGroupString;
        private readonly string _jobName;
        private readonly TimeSpan _period;

        public RecurringJobScheduler(ILogger logger, ISchedulerFactory sf, string jobGroupString,
            string jobName, TimeSpan jobPeriod) : base(logger, sf) {
            _jobGroupString = jobGroupString;
            _jobName = jobName;
            _period = jobPeriod;
        }

        public override void Schedule() {
            var sched = SF.GetScheduler();
            var basedata = new JobDataMap();

            var job = JobBuilder
                .Create<T>()
                .WithIdentity(_jobName, _jobGroupString)
                .SetJobData(basedata)
                .Build();

            Logger.LogInformation($"Scheduling recurring job {typeof(T)} to run once per {_period.TotalMinutes} minutes");

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{_jobGroupString}{_jobName}Recurring", _jobGroupString)
                .ForJob(_jobName, _jobGroupString)
                .StartAt(DateBuilder.FutureDate(30, IntervalUnit.Second))
                .WithSimpleSchedule(x => x
                    .WithInterval(_period)
                    .RepeatForever())
                .Build();

            sched.ScheduleJob(job, trigger);
        }
    }
}
