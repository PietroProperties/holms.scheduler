using System;
using Google.Protobuf.WellKnownTypes;
using HOLMS.Platform.Client;
using HOLMS.Platform.Support.Time;
using HOLMS.Types.Money.Accounting;
using HOLMS.Types.Money.RPC;
using Quartz;

namespace HOLMS.Scheduler.Jobs {
    class AccountingTransactionExportJob : QuartzJobBase {
        public const string StartDateKey = "StartDate";
        public const string EndDateKey = "EndDate";
        public const string OutputPathKey = "OutPath";
        public const string RunTypeKey = "RunType";
        public const string JobGroupString = "Reporting";
        public const string JobNameString = "AccountingTransactionExport";

        public override string JobGroup => JobGroupString;
        public override string JobName => JobNameString;
        public static TimeSpan RunAtTimeOfDay => new TimeSpan(1, 0, 0);

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac) {
            var jobStatus = ac.AccountingTxnSvc.GetExportJobStatus(new Empty());

            if (!jobStatus.DueForNextRun) {
                return;
            }

            // Export the range [last exported+1, today-1]
            var start = jobStatus.LastExportCompleted.ToDateTime().AddDays(1);
            var end = DateTime.Today.AddDays(-1);
            ac.AccountingTxnSvc.UploadPropertyTransactionsInFormat(new AccountTxnExportSvcUploadPropertyTransactionsInFormatRequest() {
                DateRange = new InclusiveCalendarDateRange(start, end).ToPB,
                UpdateLastRunDate = true,
                Format = AccountingTransactionExportFormat.ExportFormatIif
            });
        }
    }
}
