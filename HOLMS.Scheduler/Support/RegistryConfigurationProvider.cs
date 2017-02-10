using HOLMS.Scheduler.Support.Exceptions;
using Microsoft.Extensions.Logging;

namespace HOLMS.Scheduler.Support {
    public static class RegistryConfigurationProvider {
        private const string RegPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\HOLMS\SchedulerSvc";

        public static void VerifyConfiguration(ILogger l) {
            try {
                GetIIFExportDirectoryString();
            } catch(RegistrySettingNotFoundException ex) {
                l.LogError("Expected registry key not found.", ex);
                throw;
            } catch(RegistrySettingNotSetException ex) {
                l.LogError("An expected registry key has not been configured.", ex);
                throw;
            }
        }

        public static string GetIIFExportDirectoryString() {
            return NativeMethods.GetRegistryEntry(RegPath, "IIFExportPath");
        }

        public static string GetAppSvcIPPort() {
            return NativeMethods.GetRegistryEntry(RegPath, "AppSvcIPPort");
        }

        public static string GetServiceUsername() {
            return NativeMethods.GetRegistryEntry(RegPath, "ServiceUsername");
        }

        public static string GetServicePassword() {
            return NativeMethods.GetRegistryEntry(RegPath, "ServicePassword");
        }
    }
}
