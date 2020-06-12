using HOLMS.Platform.Client;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOLMS.Scheduler.Jobs
{
    class OTAOccupancyFactorPriceSyncJob : QuartzJobBase
    {
        public const string JobGroupString = "Booking";
        public const string JobNameString = "OTAOccupancyFactorPriceSyncJob";

        public static TimeSpan RunAtTimeOfDay => new TimeSpan(0, 0, 30);
        public override string JobName => JobNameString;

        public override string JobGroup => JobGroupString;

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac)
        {
            ac.OTASyncSvc.SyncPriceForOccupancyFactor(new Google.Protobuf.WellKnownTypes.Empty());
        }
    }
}
