using System;
using HOLMS.Platform.Client;
using Quartz;

namespace HOLMS.Scheduler.Jobs {
    class OTASyncJob : QuartzJobBase {
        public const string JobGroupString = "Booking";
        public const string JobNameString = "OTASync";
        public static TimeSpan JobPeriod => new TimeSpan(0, 3, 0);

        public override string JobGroup => JobGroupString;

        public override string JobName => JobNameString;

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac) {
            ac.OTASyncSvc.SyncReservations(new Google.Protobuf.WellKnownTypes.Empty());
        }
    }
}
