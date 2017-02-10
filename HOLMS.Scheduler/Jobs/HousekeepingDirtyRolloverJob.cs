using Google.Protobuf.WellKnownTypes;
using HOLMS.Application.Client;
using Quartz;

namespace HOLMS.Scheduler.Jobs {
    public class HousekeepingDirtyRolloverJob : QuartzJobBase {
        public const string JobGroupString = "HouseOps";
        public const string JobNameString = "DirtyCleanTracking";

        public override string JobGroup => JobGroupString;
        public override string JobName => JobNameString;

        public const string ImmediateRunBoolean = "ImmediateRun";
        public const int JobPeriodMins = 60;

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac) {
            if (context.MergedJobDataMap.GetBooleanValue(ImmediateRunBoolean) == false) {
                ac.HKDirtyTrackingSvc.RunUpdateJobIfScheduled(new Empty());
            } else {
                ac.HKDirtyTrackingSvc.RunUpdateJobImmediately(new Empty());
            }
        }
    }
}
