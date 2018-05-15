using System;
using HOLMS.Platform.Client;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Jobs {
    public abstract class QuartzJobBase : IJob {
        public abstract string JobName { get; }
        public abstract string JobGroup { get; }
        protected abstract void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac);

        public void Execute(IJobExecutionContext ctx) {
            try {
                var ac = Globals.AC;
                ac.Logger.LogInformation(JobName + " execution started");
                ExecuteLogged(ctx, ac);
                ac.Logger.LogInformation(JobName + " execution completed");
            } catch (Exception ex) {
                var logger = Globals.Logger;
                logger.LogError(new EventId(), ex, $"Caught unhandled exception in {JobGroup}:{JobName}");
                logger.LogError("Re-throwing to abort the job");

                throw;
            }
        }
    }
}
