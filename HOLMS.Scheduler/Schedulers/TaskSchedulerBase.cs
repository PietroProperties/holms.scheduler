using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    internal abstract class TaskSchedulerBase {
        protected readonly ISchedulerFactory SF;
        protected ILogger Logger;

        public abstract void Schedule(JobDataMap jdm);

        public void Schedule() {
            Schedule(new JobDataMap());
        }

        protected TaskSchedulerBase(ILogger logger, ISchedulerFactory scheduler) {
            SF = scheduler;
            Logger = logger;
        }
    }
}
