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

        private readonly string _outputPath;
        private InclusiveCalendarDateRange _reportRange;

        public AccountingTransactionExportScheduler(ILogger logger, ISchedulerFactory sf)
                : base(logger, sf) {
            _outputPath = RegistryConfigurationProvider.GetIIFExportDirectoryString();
        }

        public override void ParseCommandLineArgs(string[] args) {
            // Parse command line arguments for starting the service to create report immediately
            // syntax is: --immediatereport <start date> <end date> where the start and end dates
            // are endpoints of an inclusive date range of which to produce reports. The date format
            // is mm/dd/yyyy.
            Logger.LogInformation("Parsing arguments in AccountingTransactionExportJobScheduler");

            Logger.LogInformation("Using nightly reporting schedule");
        }

        public override void Schedule() {
            Logger.LogInformation("Scheduling nightly reporting run for accounting transaction reporter");
            ScheduleNightlyReport();
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
