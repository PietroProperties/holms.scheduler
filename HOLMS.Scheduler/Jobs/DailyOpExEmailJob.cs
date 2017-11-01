using Google.Protobuf.WellKnownTypes;
using HOLMS.Application.Client;
using Quartz;

namespace HOLMS.Scheduler.Jobs {
    public class DailyOpExEmailJob : QuartzJobBase {
        public const string JobGroupString = "Administration";
        public const string JobNameString = "DailyOpExEmail";

        public const int JobPeriodHours = 24;

        public override string JobGroup => JobGroupString;
        public override string JobName => JobNameString;

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac) {
            ac.OperationsReportingSvc.SendOpExReportEmailForLastDay(new Empty());
        }
    }
}
