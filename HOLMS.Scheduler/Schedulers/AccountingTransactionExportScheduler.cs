using HOLMS.Scheduler.Jobs;
using HOLMS.Scheduler.Support;
using Microsoft.Extensions.Logging;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    class AccountingTransactionExportScheduler : TaskSchedulerBase {

        private readonly string _outputPath;

        public AccountingTransactionExportScheduler(ILogger logger, ISchedulerFactory sf)
                : base(logger, sf) {
            _outputPath = RegistryConfigurationProvider.GetIIFExportDirectoryString();
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
