﻿using System;
using System.Diagnostics;
using HOLMS.Scheduler.Schedulers;
using HOLMS.Scheduler.Support;
using Quartz;
using Quartz.Collection;
using Quartz.Impl;
using System.Linq;
using System.ServiceProcess;
using Google.Protobuf.WellKnownTypes;
using HOLMS.Scheduler.Jobs;
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
                Debugger.Launch();
            }
            var logger = Globals.Logger;
            RegistryConfigurationProvider.VerifyConfiguration(logger);

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            logger.LogInformation($"Starting HOLMS Scheduler Service {fvi.FileVersion}. Creating tasks.");
            
            // Run every n minutes
            _jobSchedulers.Add(new RecurringJobScheduler<DailyOpExEmailJob>(logger, _schedulerFactory,
                DailyOpExEmailJob.JobGroupString, DailyOpExEmailJob.JobNameString,
                DailyOpExEmailJob.JobPeriod));
            _jobSchedulers.Add(new RecurringJobScheduler<HousekeepingDirtyRolloverJob>(logger,
                _schedulerFactory, HousekeepingDirtyRolloverJob.JobGroupString,
                HousekeepingDirtyRolloverJob.JobNameString, HousekeepingDirtyRolloverJob.JobPeriod));
            _jobSchedulers.Add(new RecurringJobScheduler<OTASyncJob>(logger, _schedulerFactory,
                OTASyncJob.JobGroupString, OTASyncJob.JobNameString, OTASyncJob.JobPeriod));
            _jobSchedulers.Add(new RecurringJobScheduler<SupplyHistorySnapshotJob>(
                logger, _schedulerFactory, SupplyHistorySnapshotJob.JobGroupString,
                SupplyHistorySnapshotJob.JobNameString, SupplyHistorySnapshotJob.JobPeriod));
            _jobSchedulers.Add(new RecurringJobScheduler<OTATimeFactorPriceSyncJob>(
               logger, _schedulerFactory, OTATimeFactorPriceSyncJob.JobGroupString,
               OTATimeFactorPriceSyncJob.JobNameString, OTATimeFactorPriceSyncJob.JobPeriod));


            // Run at same time every day
            _jobSchedulers.Add(new FixedTimeOfDayScheduler<AccountingTransactionExportJob>(
                logger, _schedulerFactory,
                AccountingTransactionExportJob.JobGroupString,
                AccountingTransactionExportJob.JobNameString,
                AccountingTransactionExportJob.RunAtTimeOfDay));
            _jobSchedulers.Add(new FixedTimeOfDayScheduler<AuthorizationCleanupJob>(
                logger, _schedulerFactory, AuthorizationCleanupJob.JobGroupString,
                AuthorizationCleanupJob.JobNameString, AuthorizationCleanupJob.RunAtTimeOfDay));
            _jobSchedulers.Add(new FixedTimeOfDayScheduler<GuaranteeAuthorizerJob>(
                logger, _schedulerFactory, GuaranteeAuthorizerJob.JobGroupString,
                GuaranteeAuthorizerJob.JobNameString, GuaranteeAuthorizerJob.RunAtTimeOfDay));
            _jobSchedulers.Add(new FixedTimeOfDayScheduler<OTAOccupancyFactorPriceSyncJob>(
                logger, _schedulerFactory, OTAOccupancyFactorPriceSyncJob.JobGroupString,
                OTAOccupancyFactorPriceSyncJob.JobNameString, OTAOccupancyFactorPriceSyncJob.RunAtTimeOfDay));



            logger.LogInformation("Scheduling tasks");
            foreach (var t in _jobSchedulers) {
                t.Schedule();
            }

            logger.LogInformation("Scheduling rollover chime job");
            var propResp = Globals.AC.PropertySvc.All(new Empty());
            foreach (var p in propResp.Properties) {
                var checkinTs = p.CheckinTimeOfDay.ToTimeSpan();
                logger.LogInformation($"Rollover chimes 1 minute after checkin time: {checkinTs}. desc={p.Description}");
                var s = new FixedTimeOfDayScheduler<CheckinChimeJob>(logger, _schedulerFactory,
                    CheckinChimeJob.JobGroup,
                    CheckinChimeJob.JobName(p.EntityId),
                    // Run 1 minute after checkin
                    // https://www.youtube.com/watch?v=2AKhvbBuvVw
                    checkinTs + new TimeSpan(0, 0, 1, 0));
                s.Schedule(CheckinChimeJob.MakeMap(p.EntityId));
            }

            logger.LogInformation("Starting main execution loop");
            var scheduler = _schedulerFactory.GetScheduler();
            scheduler.Start();
        }

        protected override void OnStop() {
            var logger = Globals.Logger;
            logger.LogInformation("Stopping service");

            var scheduler = _schedulerFactory.GetScheduler();
            scheduler.Shutdown();
        }
    }
}
