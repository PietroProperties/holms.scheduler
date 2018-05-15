using System;
using Google.Protobuf.WellKnownTypes;
using HOLMS.Platform.Client;
using Quartz;

namespace HOLMS.Scheduler.Jobs {
    public class HousekeepingDirtyRolloverJob : QuartzJobBase {
        public const string JobGroupString = "HouseOps";
        public const string JobNameString = "DirtyCleanTracking";
        public static TimeSpan JobPeriod => new TimeSpan(1, 0, 0);

        public override string JobGroup => JobGroupString;
        public override string JobName => JobNameString;

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac) {
            ac.HKDirtyTrackingSvc.RunUpdateJobIfScheduled(new Empty());
        }
    }
}
