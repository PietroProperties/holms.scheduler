using Microsoft.Extensions.Logging;

namespace HOLMS.Scheduler.Tests {
    class TestLogger {
        public static ILogger GetLogger() {
            var lf = new LoggerFactory();
            lf.AddConsole();

            return lf.CreateLogger("Scheduler");
        }
    }
}
