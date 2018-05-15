using System;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    class FixedTimeOfDayScheduler<T> : TaskSchedulerBase where T : IJob {
        private readonly string _jobGroupString;
        private readonly string _jobName;
        private readonly TimeSpan _timeOfDay;

        public FixedTimeOfDayScheduler(ILogger logger, ISchedulerFactory sf, string jobGroupString,
            string jobName, TimeSpan jobTimeOfDay) : base(logger, sf) {
            _jobGroupString = jobGroupString;
            _jobName = jobName;
            _timeOfDay = jobTimeOfDay;
        }

        public override void Schedule(JobDataMap jdm) {
            var sched = SF.GetScheduler();

            var job = JobBuilder
                .Create<T>()
                .WithIdentity(_jobName, _jobGroupString)
                .SetJobData(jdm)
                .Build();

            Logger.LogInformation($"Scheduling fixed time of day job {typeof(T)} to run every day at {_timeOfDay.TotalHours} hours past midnight system time");

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{_jobGroupString}{_jobName}FixedTimeOfDay", _jobGroupString)
                .ForJob(_jobName, _jobGroupString)
                .StartAt(DateBuilder.FutureDate(30, IntervalUnit.Second))
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(_timeOfDay.Hours, _timeOfDay.Minutes))
                .Build();

            sched.ScheduleJob(job, trigger);
        }
    }
}
