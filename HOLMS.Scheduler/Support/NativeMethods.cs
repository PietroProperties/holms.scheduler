using System;
using HOLMS.Scheduler.Support.Exceptions;
using Microsoft.Win32;

namespace HOLMS.Scheduler.Support {
    internal class NativeMethods {
        public static string GetRegistryEntry(string keypath, string valuename) {
            var keyvalue = (string)Registry.GetValue(keypath, valuename, null);
            if (keyvalue == "NotSet") {
                throw new RegistrySettingNotSetException(keypath, valuename);
            }
            if (keyvalue == null) {
                throw new RegistrySettingNotFoundException(keypath, valuename);
            }
            return keyvalue;
        }

        public static Guid GetWindowsMachineGuid() {
            // With gratitute to http://www.nextofwindows.com/the-best-way-to-uniquely-identify-a-windows-machine
            // This will be the same for every user, and won't change until/unless windows is reinstalled
            var mgstr = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Cryptography",
                "MachineGuid", null);
            return new Guid(mgstr);
        }
    }
}
