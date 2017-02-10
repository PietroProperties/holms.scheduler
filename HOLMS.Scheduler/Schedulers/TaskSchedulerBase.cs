using HOLMS.Application.Client;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    internal abstract class TaskSchedulerBase {
        protected readonly IApplicationClient DC;
        protected readonly ISchedulerFactory SF;

        protected ILogger Logger => DC.Logger;

        public abstract void ParseCommandLineArgs(string[] args);

        public abstract void Schedule();

        protected TaskSchedulerBase(IApplicationClient ac, ISchedulerFactory scheduler) {
            DC = ac;
            SF = scheduler;
        }
    }
}
