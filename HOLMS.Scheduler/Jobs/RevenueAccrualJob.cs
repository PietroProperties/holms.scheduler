using Google.Protobuf.WellKnownTypes;
using HOLMS.Application.Client;
using HOLMS.Types.TenancyConfig.RPC;
using Quartz;

namespace HOLMS.Scheduler.Jobs {
    class RevenueAccrualJob : QuartzJobBase {
        public const string JobGroupString = "Reporting";
        public const string JobNameString = "RevenueAccrual";

        public override string JobGroup => JobGroupString;
        public override string JobName => JobNameString;

        public const int JobPeriodMins = 120;

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac) {
            var properties = ac.PropertySvc.All(new Empty()).Properties;
            foreach (var p in properties) {
                var accrualReq = new PropertySvcAccrueRevenueRequest {
                    Property = p.EntityId,
                    RunEveryMins = JobPeriodMins
                };

                ac.PropertySvc.AccrueRevenue(accrualReq);
            }
        }
    }
}
