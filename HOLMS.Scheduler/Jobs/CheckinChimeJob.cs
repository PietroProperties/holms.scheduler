using System;
using System.Collections.Generic;
using HOLMS.Types.TenancyConfig.Indicators;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Jobs {
    class CheckinChimeJob : IJob {
        private const string PropIndicatorKey = "pi";
        
        public static string JobGroup => "TenancyConfig";
        public static string JobName(PropertyIndicator pi) => $"CheckinChime.{pi.GuidID}";

        public static JobDataMap MakeMap(PropertyIndicator pi) =>
            new JobDataMap(new Dictionary<string, string> {
                {PropIndicatorKey, pi.GuidID.ToString()}
            });

        public PropertyIndicator GetProp(IJobExecutionContext ctx) =>
            new PropertyIndicator(new Guid((string)ctx.Get(PropIndicatorKey)));

        public void Execute(IJobExecutionContext ctx) {
            var pi = GetProp(ctx);

            try {    
                var ac = Globals.AC;
                ac.Logger.LogInformation(JobName(pi) + " execution started");
                ac.Logger.LogInformation($"Checkin chime. pi={pi.GuidID}");
                ac.Logger.LogInformation(JobName(pi) + " execution completed");
            } catch (Exception ex) {
                var logger = Globals.Logger;
                logger.LogError(new EventId(), ex, $"Caught unhandled exception in {JobGroup}:{JobName(pi)}");
                logger.LogError("Re-throwing to abort the job");

                throw;
            }
        }
    }
}
