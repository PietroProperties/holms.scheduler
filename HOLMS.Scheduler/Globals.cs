using HOLMS.Application.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOLMS.Scheduler {
    public static class Globals {
        public static void BuildApplicationClient() {
            _ac = JobEnvConstructor.GetAppServiceClient();
        }

        static ApplicationClient _ac;

        public static ApplicationClient AC {
            get {
                return _ac;
            }
        }
    }
}
