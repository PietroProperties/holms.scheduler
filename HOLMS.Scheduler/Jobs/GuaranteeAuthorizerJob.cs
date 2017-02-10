using Google.Protobuf.WellKnownTypes;
using HOLMS.Application.Client;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Jobs {
    public class GuaranteeAuthorizerJob : QuartzJobBase {
        public const string JobGroupString = "Guarantees";
        public const string JobNameString = "GuaranteeAuthorizer";

        public override string JobGroup => JobGroupString;
        public override string JobName => JobNameString;

        public const int JobPeriodMins = 1440;

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient dc) {
            dc.Logger.LogInformation($"Beginning guarantee authorizer job for tenancy {dc.SC.TenancyName}");
            dc.GuaranteeAuthorizerService.AuthorizeForTenancy(new Empty());
        }
    }
}
