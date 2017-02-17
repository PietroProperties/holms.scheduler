using HOLMS.Scheduler.Schedulers;
using HOLMS.Scheduler.Support;
using Quartz;
using Quartz.Collection;
using Quartz.Impl;
using System.Linq;
using System.ServiceProcess;
using Microsoft.Extensions.Logging;

namespace HOLMS.Scheduler {
    public class SchedulerService : ServiceBase {
        private readonly HashSet<TaskSchedulerBase> _jobSchedulers;
        private readonly ISchedulerFactory _schedulerFactory;

        public SchedulerService() {
            ServiceName = "HOLMSSchedulerService";
            _jobSchedulers = new HashSet<TaskSchedulerBase>();
            _schedulerFactory = new StdSchedulerFactory();
        }

        protected override void OnStart(string[] args) {
            if (args.Contains("--debugger")) {
                System.Diagnostics.Debugger.Launch();
            }
            Globals.BuildApplicationClient();
            var logger = JobEnvConstructor.GetLogger();
            RegistryConfigurationProvider.VerifyConfiguration(logger);
            var ac = JobEnvConstructor.GetAppServiceClient();

            ac.Logger.LogInformation("SchedulerService starting. Creating tasks");
            _jobSchedulers.Add(new AccountingTransactionExportScheduler(logger, _schedulerFactory));
            _jobSchedulers.Add(new RevenueAccrualScheduler(logger, _schedulerFactory));
            _jobSchedulers.Add(new HousekeepingDirtyRolloverScheduler(logger, _schedulerFactory));
            _jobSchedulers.Add(new GuaranteeAuthorizerScheduler(logger, _schedulerFactory));
            _jobSchedulers.Add(new OTASyncScheduler(logger, _schedulerFactory));

            ac.Logger.LogInformation("Passing command-line arguments to tasks");
            foreach (var t in _jobSchedulers) {
                t.ParseCommandLineArgs(args);
            }

            ac.Logger.LogInformation("Scheduling tasks");
            foreach (var t in _jobSchedulers) {
                t.Schedule();
            }

            ac.Logger.LogInformation("Starting main execution loop");
            var scheduler = _schedulerFactory.GetScheduler();
            scheduler.Start();
        }

        protected override void OnStop() {
            var logger = JobEnvConstructor.GetLogger();
            logger.LogInformation("Stopping service");

            var scheduler = _schedulerFactory.GetScheduler();
            scheduler.Shutdown();
        }
    }
}
