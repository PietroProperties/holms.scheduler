using System;
using HOLMS.Scheduler.Jobs;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    class FixedTimeOfDayScheduler<T> : TaskSchedulerBase where T : QuartzJobBase {
        private readonly string _jobGroupString;
        private readonly string _jobName;
        private readonly TimeSpan _timeOfDay;

        public FixedTimeOfDayScheduler(ILogger logger, ISchedulerFactory sf, string jobGroupString,
            string jobName, TimeSpan jobTimeOfDay) : base(logger, sf) {
            _jobGroupString = jobGroupString;
            _jobName = jobName;
            _timeOfDay = jobTimeOfDay;
        }

        public override void Schedule() {
            var sched = SF.GetScheduler();
            var basedata = new JobDataMap();

            var job = JobBuilder
                .Create<T>()
                .WithIdentity(_jobName, _jobGroupString)
                .SetJobData(basedata)
                .Build();

            Logger.LogInformation($"Scheduling fixed time of day job {typeof(T)} to run every day at {_timeOfDay.TotalHours} past midnight system time");

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{_jobGroupString}{_jobName}Periodic", _jobGroupString)
                .ForJob(job.Key)
                .StartAt(DateBuilder.FutureDate(30, IntervalUnit.Second))
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(_timeOfDay.Hours, _timeOfDay.Minutes))
                .Build();

            sched.ScheduleJob(trigger);
        }

    }
}
