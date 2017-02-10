using System;
using System.Collections.Generic;
using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;

namespace HOLMS.Scheduler.Tests {
    class SchedulerFake : IScheduler {
        public int ScheduleJobCallCount;
        public IJobDetail ScheduleJobJobDetail;
        public ITrigger ScheduleJobTrigger;

        public DateTimeOffset ScheduleJob(IJobDetail jobDetail, ITrigger trigger) {
            ScheduleJobCallCount++;
            ScheduleJobJobDetail = jobDetail;
            ScheduleJobTrigger = trigger;

            return new DateTimeOffset();
        }

        public bool IsJobGroupPaused(string groupName) {
            throw new NotImplementedException();
        }

        public bool IsTriggerGroupPaused(string groupName) {
            throw new NotImplementedException();
        }

        public SchedulerMetaData GetMetaData() {
            throw new NotImplementedException();
        }

        public IList<IJobExecutionContext> GetCurrentlyExecutingJobs() {
            throw new NotImplementedException();
        }

        public IList<string> GetJobGroupNames() {
            throw new NotImplementedException();
        }

        public IList<string> GetTriggerGroupNames() {
            throw new NotImplementedException();
        }

        public Quartz.Collection.ISet<string> GetPausedTriggerGroups() {
            throw new NotImplementedException();
        }

        public void Start() {
            throw new NotImplementedException();
        }

        public void StartDelayed(TimeSpan delay) {
            throw new NotImplementedException();
        }

        public void Standby() {
            throw new NotImplementedException();
        }

        public void Shutdown() {
            throw new NotImplementedException();
        }

        public void Shutdown(bool waitForJobsToComplete) {
            throw new NotImplementedException();
        }

        public DateTimeOffset ScheduleJob(ITrigger trigger) {
            throw new NotImplementedException();
        }

        public void ScheduleJobs(IDictionary<IJobDetail, Quartz.Collection.ISet<ITrigger>> triggersAndJobs, bool replace) {
            throw new NotImplementedException();
        }

        public void ScheduleJob(IJobDetail jobDetail, Quartz.Collection.ISet<ITrigger> triggersForJob, bool replace) {
            throw new NotImplementedException();
        }

        public bool UnscheduleJob(TriggerKey triggerKey) {
            throw new NotImplementedException();
        }

        public bool UnscheduleJobs(IList<TriggerKey> triggerKeys) {
            throw new NotImplementedException();
        }

        public DateTimeOffset? RescheduleJob(TriggerKey triggerKey, ITrigger newTrigger) {
            throw new NotImplementedException();
        }

        public void AddJob(IJobDetail jobDetail, bool replace) {
            throw new NotImplementedException();
        }

        public void AddJob(IJobDetail jobDetail, bool replace, bool storeNonDurableWhileAwaitingScheduling) {
            throw new NotImplementedException();
        }

        public bool DeleteJob(JobKey jobKey) {
            throw new NotImplementedException();
        }

        public bool DeleteJobs(IList<JobKey> jobKeys) {
            throw new NotImplementedException();
        }

        public void TriggerJob(JobKey jobKey) {
            throw new NotImplementedException();
        }

        public void TriggerJob(JobKey jobKey, JobDataMap data) {
            throw new NotImplementedException();
        }

        public void PauseJob(JobKey jobKey) {
            throw new NotImplementedException();
        }

        public void PauseJobs(GroupMatcher<JobKey> matcher) {
            throw new NotImplementedException();
        }

        public void PauseTrigger(TriggerKey triggerKey) {
            throw new NotImplementedException();
        }

        public void PauseTriggers(GroupMatcher<TriggerKey> matcher) {
            throw new NotImplementedException();
        }

        public void ResumeJob(JobKey jobKey) {
            throw new NotImplementedException();
        }

        public void ResumeJobs(GroupMatcher<JobKey> matcher) {
            throw new NotImplementedException();
        }

        public void ResumeTrigger(TriggerKey triggerKey) {
            throw new NotImplementedException();
        }

        public void ResumeTriggers(GroupMatcher<TriggerKey> matcher) {
            throw new NotImplementedException();
        }

        public void PauseAll() {
            throw new NotImplementedException();
        }

        public void ResumeAll() {
            throw new NotImplementedException();
        }

        public Quartz.Collection.ISet<JobKey> GetJobKeys(GroupMatcher<JobKey> matcher) {
            throw new NotImplementedException();
        }

        public IList<ITrigger> GetTriggersOfJob(JobKey jobKey) {
            throw new NotImplementedException();
        }

        public Quartz.Collection.ISet<TriggerKey> GetTriggerKeys(GroupMatcher<TriggerKey> matcher) {
            throw new NotImplementedException();
        }

        public IJobDetail GetJobDetail(JobKey jobKey) {
            throw new NotImplementedException();
        }

        public ITrigger GetTrigger(TriggerKey triggerKey) {
            throw new NotImplementedException();
        }

        public TriggerState GetTriggerState(TriggerKey triggerKey) {
            throw new NotImplementedException();
        }

        public void AddCalendar(string calName, ICalendar calendar, bool replace, bool updateTriggers) {
            throw new NotImplementedException();
        }

        public bool DeleteCalendar(string calName) {
            throw new NotImplementedException();
        }

        public ICalendar GetCalendar(string calName) {
            throw new NotImplementedException();
        }

        public IList<string> GetCalendarNames() {
            throw new NotImplementedException();
        }

        public bool Interrupt(JobKey jobKey) {
            throw new NotImplementedException();
        }

        public bool Interrupt(string fireInstanceId) {
            throw new NotImplementedException();
        }

        public bool CheckExists(JobKey jobKey) {
            throw new NotImplementedException();
        }

        public bool CheckExists(TriggerKey triggerKey) {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        public string SchedulerName { get; }
        public string SchedulerInstanceId { get; }
        public SchedulerContext Context { get; }
        public bool InStandbyMode { get; }
        public bool IsShutdown { get; }
        public IJobFactory JobFactory { get; set; }
        public IListenerManager ListenerManager { get; }
        public bool IsStarted { get; }
    }
}
