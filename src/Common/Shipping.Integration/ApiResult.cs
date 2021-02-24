using System;

namespace Common.Shipping.Integration
{
    public class ApiResult
    {
        public string ErrorMessage { get; private set; }

        public string SuccessMessage { get; private set; }
        public string TrackingNumber { get; private set; }
        public bool Sucsess { get; private set; }

        public bool Failed { get; private set; }
        public bool Redirect { get; set; }

        public ApiResult RequestFailed(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Failed = true;
            return this;
        }

        public void RequestPassed(string successMessage, string trackingNumber)
        {
            Sucsess = true;
            SuccessMessage = successMessage;
            TrackingNumber = trackingNumber;
        }

        public ApiResult ForceFail(string error)
        {
            throw new FormatException(error);
        }
    }
}