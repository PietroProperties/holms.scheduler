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

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac) {
            ac.Logger.LogInformation($"Beginning guarantee authorizer job for tenancy {ac.SC.TenancyName}");
            ac.GuaranteeAuthorizerService.AuthorizeForTenancy(new Empty());
        }
    }
}
