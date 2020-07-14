using HOLMS.Platform.Client;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOLMS.Scheduler.Jobs
{
    class OTATimeFactorPriceSyncJob : QuartzJobBase
    {
        public const string JobGroupString = "Booking";
        public const string JobNameString = "OTATimeFactorPriceSyncJob";
        public static TimeSpan JobPeriod => new TimeSpan(1,0, 0);


        public override string JobName => JobNameString;

        public override string JobGroup => JobGroupString;

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac)
        {
            ac.OTASyncSvc.SyncPriceForTimeFactor(new Google.Protobuf.WellKnownTypes.Empty());
        }
    }
}
