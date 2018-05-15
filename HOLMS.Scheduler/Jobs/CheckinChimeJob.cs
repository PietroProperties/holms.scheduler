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
            new PropertyIndicator(new Guid(ctx.MergedJobDataMap.GetString(PropIndicatorKey)));

        public void Execute(IJobExecutionContext ctx) {
            var pi = GetProp(ctx);

            try {    
                var ac = Globals.AC;
                ac.Logger.LogInformation($"Checkin chime (opsday start). pi={pi.GuidID}");
                ac.ChimeSvcClient.ChimeOpsdayStart(pi);
            } catch (Exception ex) {
                var logger = Globals.Logger;
                logger.LogError(new EventId(), ex, $"Caught unhandled exception in {JobGroup}:{pi.GuidID}");
                logger.LogError("Re-throwing to abort the job");

                throw;
            }
        }
    }
}
