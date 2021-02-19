using System;

namespace Common.Shipping.Integration
{
    public class ApiResult
    {
        public string Error { get; private set; }

        public string PassInfo { get; private set; }

        public bool Pass { get; private set; }

        public bool Failed { get; private set; }

        public ApiResult RequestFailed(string error)
        {
            Error = error;
            Failed = true;
            return this;
        }

        public void RequestPassed(string passMessage)
        {
            Pass = true;
            PassInfo = passMessage;
        }

        public ApiResult ForceFail(string error)
        {
            throw new FormatException(error);
        }
    }
}