using HOLMS.Application.Client;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    internal abstract class TaskSchedulerBase {
        protected readonly ISchedulerFactory SF;
        protected ILogger Logger;

        public abstract void Schedule();

        protected TaskSchedulerBase(ILogger logger, ISchedulerFactory scheduler) {
            SF = scheduler;
            Logger = logger;
        }
    }
}
