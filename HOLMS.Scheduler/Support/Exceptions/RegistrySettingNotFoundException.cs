using System;

namespace HOLMS.Scheduler.Support.Exceptions {
    public  class RegistrySettingNotFoundException : Exception {
        public RegistrySettingNotFoundException(string ExpectedKey, string ExpectedValueName) : 
            base(ExceptionMessage(ExpectedKey, ExpectedValueName)) { }

        private static string ExceptionMessage(string ExpectedKey, string ExpectedValueName) {
            return $"Expected registry key not found at {ExpectedKey}, with name {ExpectedValueName}. ";
        }
    }
}
