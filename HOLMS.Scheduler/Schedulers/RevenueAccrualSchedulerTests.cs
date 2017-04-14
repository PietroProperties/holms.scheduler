using HOLMS.Application.Client;
using HOLMS.Scheduler.Jobs;
using HOLMS.Scheduler.Tests;
using Moq;
using NUnit.Framework;
using Quartz;

namespace HOLMS.Scheduler.Schedulers {
    class RevenueAccrualSchedulerTests {
        private Mock<ISchedulerFactory> _sf;
        private SchedulerFake _sched;
        private MockApplicationClient _mac;

        [SetUp]
        public void Init() {
            _sched = new SchedulerFake();

            _sf = new Mock<ISchedulerFactory>();
            _sf.Setup(x => x.GetScheduler()).Returns(_sched);

            var log = TestLogger.GetLogger();
            _mac = new MockApplicationClient(log);
        }

        [Test]
        public void DefaultArgumentsSchedulesTaskAndJob() {
            var ras = new RevenueAccrualScheduler(_mac.Logger, _sf.Object);

            Assert.AreEqual(0, _sched.ScheduleJobCallCount);
            ras.Schedule();
            Assert.AreEqual(1, _sched.ScheduleJobCallCount);
            // Not the most satisfying test case -- we want to check the scheduling interval,
            // but Quartz's objects don't make it easy.
            Assert.AreEqual("RevenueAccrualPeriodic", _sched.ScheduleJobTrigger.Key.Name);
            Assert.AreEqual(RevenueAccrualJob.JobGroupString, _sched.ScheduleJobTrigger.Key.Group);
        }
    }
}
