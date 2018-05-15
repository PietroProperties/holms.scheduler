using System;
using Google.Protobuf.WellKnownTypes;
using HOLMS.Platform.Client;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Jobs {
    class AuthorizationCleanupJob : QuartzJobBase {
        public const string JobGroupString = "AuthorizationCleanup";
        public const string JobNameString = "AuthorizationCleanupJob";

        public override string JobGroup => JobGroupString;
        public override string JobName => JobNameString;
        public static TimeSpan RunAtTimeOfDay => new TimeSpan(2, 30, 0);

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac) {
            ac.Logger.LogInformation($"Beginning card authorization cleanup job for tenancy {ac.SC.TenancyName}");
            ac.FolioAuthCleanupSvc.VoidAuthorizationsForDepartedReservations(new Empty());
        }
    }
}
