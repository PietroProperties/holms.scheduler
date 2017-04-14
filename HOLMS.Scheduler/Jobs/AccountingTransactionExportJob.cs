using System;
using System.IO;
using Google.Protobuf.WellKnownTypes;
using HOLMS.Application.Client;
using HOLMS.Support.Conversions;
using HOLMS.Support.Time;
using HOLMS.Types.Money.Accounting;
using HOLMS.Types.Money.RPC;
using HOLMS.Types.TenancyConfig;
using Microsoft.Extensions.Logging;
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

        protected override void ExecuteLogged(IJobExecutionContext context, ApplicationClient ac) {
            var jobStatus = ac.AccountingTxnSvc.GetExportJobStatus(new Empty());

            if (!jobStatus.DueForNextRun) {
                return;
            }

            // Export the range [last exported+1, today-1]
            var start = jobStatus.LastExportCompleted.ToDateTime().AddDays(1);
            var end = DateTime.Today.AddDays(-1);
            var pathString = context.JobDetail.JobDataMap.GetString(OutputPathKey);

            WriteReport(ac, new InclusiveCalendarDateRange(start, end), pathString);

            ac.AccountingTxnSvc.UpdateLastExportRunTime(end.ToTS());
        }


        private void WriteReport(ApplicationClient ac, InclusiveCalendarDateRange dr, string pathString) {
            var properties = ac.PropertySvc.All(new Empty()).Properties;

            foreach (var prop in properties) {
                var req = new AccountTxnExportSvcGetPropertyTransactionsInFormatRequest {
                    Property = prop.EntityId,
                    DateRange = dr.ToPB,
                    Format = AccountingTransactionExportFormat.ExportFormatIif
                };

                var iifBlob = ac.AccountingTxnSvc.GetPropertyTransactionsInFormat(req);

                var filenameString = Path.Combine(pathString, MakeExportFilename(prop, dr));
                var fileinfo = new FileInfo(filenameString);
                fileinfo.Directory.Create();

                File.WriteAllBytes(fileinfo.FullName, iifBlob.ExportedTransactions.ToByteArray());
            }
        }

        private string MakeExportFilename(Property prop, InclusiveCalendarDateRange dr) {
            var filestring = prop.Description.Replace(" ", string.Empty) +
                dr.StartDateTime.ToString(@"MM-dd") +
                "--" +
                dr.EndDateTime.ToString(@"MM-dd") +
                ".iif";
            // Clean the string of any invalid characters so that mr. smartypants at the front
            // desk can't crash the service by naming the property <>?HO||ow
            foreach (char c in Path.GetInvalidFileNameChars()) {
                filestring.Replace(c.ToString(), "");
            }
            return filestring;
        }
    }
}
