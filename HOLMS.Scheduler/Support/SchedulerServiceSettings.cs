using System;
using HOLMS.Platform.Client;
using Microsoft.Extensions.Logging;

namespace HOLMS.Scheduler.Support {
    class SchedulerServiceSettings : IApplicationClientConfig {
        public SchedulerServiceSettings(ILogger l) {
            var appSvcIpPort = RegistryConfigurationProvider.GetAppSvcIPPort();
            var tokens = appSvcIpPort.Split(":".ToCharArray());

            if (tokens == null || tokens.Length != 2) {
                throw new Exception("Missing or incorrect application server hostname and port");
            }

            AppSvcHostname = tokens[0];

            ushort appSvcPort;
            AppSvcHostname = tokens[0];
            if (!ushort.TryParse(tokens[1], out appSvcPort)) {
                l.LogError($"Failed to parse AppSvcIPPort TCP port {tokens[1]} as valid port number (integer between 1 and 65535");
                throw new ArgumentException();
            }
            AppSvcPort = appSvcPort;

            ClientInstanceId = NativeMethods.GetWindowsMachineGuid();
        }

        public string AppSvcHostname { get; }
        public ushort AppSvcPort { get; }

        public Guid ClientInstanceId { get; }
        public string GetServiceUsername() => RegistryConfigurationProvider.GetServiceUsername();
        public string GetServicePassword() => RegistryConfigurationProvider.GetServicePassword();
    }
}