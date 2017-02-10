using System.ServiceProcess;

namespace HOLMS.Scheduler
{
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var services = new ServiceBase[] {
                new SchedulerService()
            };
            ServiceBase.Run(services);
        }
    }
}
