using System;
using System.Collections.Generic;
using System.Linq;
using HOLMS.Application.Client;
using HOLMS.Scheduler.Jobs;
using HOLMS.Scheduler.Support;
using HOLMS.Support.Time;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    class AccountingTransactionExportScheduler : TaskSchedulerBase {
        const string ImmediateReportFlag = "--immediatereport";

        private readonly string _outputPath;
        private InclusiveCalendarDateRange _reportRange;
        private bool _immediateReportRequested;

        public AccountingTransactionExportScheduler(IApplicationClient ac, ISchedulerFactory sf)
                : base(ac, sf) {
            _outputPath = RegistryConfigurationProvider.GetIIFExportDirectoryString();
        }

        public override void ParseCommandLineArgs(string[] args) {
            // Parse command line arguments for starting the service to create report immediately
            // syntax is: --immediatereport <start date> <end date> where the start and end dates
            // are endpoints of an inclusive date range of which to produce reports. The date format
            // is mm/dd/yyyy.
            Logger.LogInformation("Parsing arguments in AccountingTransactionExportJobScheduler");

            if (args.Contains(ImmediateReportFlag)) {
                _immediateReportRequested = true;
                _reportRange = ParseImmediateReportRange(args);
                Logger.LogInformation($"Immediate report requested for date range: {_reportRange.Start} to {_reportRange.End}");
            } else {
                _immediateReportRequested = false;
                Logger.LogInformation("Using nightly reporting schedule");
            }
        }

        public override void Schedule() {
            if (_immediateReportRequested) {
                Logger.LogInformation("Scheduling immediate reporting run for accounting transaction reporter");
                ScheduleImmediateReport();
            } else {
                Logger.LogInformation("Scheduling nightly reporting run for accounting transaction reporter");
                ScheduleNightlyReport();
            }
        }

        private static InclusiveCalendarDateRange ParseImmediateReportRange(IReadOnlyList<string> args) {
            DateTime start;
            DateTime end;
            int i = args.ToList().IndexOf(ImmediateReportFlag);

            if (DateTime.TryParse(args[i + 1], out start) && DateTime.TryParse(args[i + 2], out end)) {
                return new InclusiveCalendarDateRange(start, end);
            } else {
                return new InclusiveCalendarDateRange(DateTime.Today, DateTime.Today);
            }
        }

        private void ScheduleImmediateReport() {
            var basedata = new JobDataMap() {
                {AccountingTransactionExportJob.OutputPathKey, _outputPath},
                {AccountingTransactionExportJob.RunTypeKey, "Manual" }
            };

            var datedata = new JobDataMap() {
                {AccountingTransactionExportJob.EndDateKey, _reportRange.EndDateTime},
                {AccountingTransactionExportJob.StartDateKey, _reportRange.StartDateTime}
            };

            var job = JobBuilder
                .Create<AccountingTransactionExportJob>()
                .WithIdentity(AccountingTransactionExportJob.JobNameString,
                    AccountingTransactionExportJob.JobGroupString)
                .SetJobData(basedata)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("AccountingTransactionExportImmediate",
                    AccountingTransactionExportJob.JobGroupString)
                .ForJob(AccountingTransactionExportJob.JobNameString,
                    AccountingTransactionExportJob.JobGroupString)
                .UsingJobData(datedata)
                .WithSimpleSchedule(x => x.WithInterval(new TimeSpan(0, 0, 0)).WithRepeatCount(0))
                .Build();

            var sched = SF.GetScheduler();
            sched.ScheduleJob(job, trigger);
        }

        private void ScheduleNightlyReport() {
            var basedata = new JobDataMap() {
                {AccountingTransactionExportJob.OutputPathKey, _outputPath},
                {AccountingTransactionExportJob.RunTypeKey, "Auto" }
            };

            var job = JobBuilder
                .Create<AccountingTransactionExportJob>()
                .WithIdentity(AccountingTransactionExportJob.JobNameString,
                    AccountingTransactionExportJob.JobGroupString)
                .SetJobData(basedata)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("AccountingTransactionExportPeriodic",
                    AccountingTransactionExportJob.JobGroupString)
                .ForJob(AccountingTransactionExportJob.JobNameString,
                    AccountingTransactionExportJob.JobGroupString)
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(1, 0))
                .Build();

            var sched = SF.GetScheduler();
            sched.ScheduleJob(job, trigger);
        }
    }
}
