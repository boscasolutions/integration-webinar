using System;
using Shipping.Integration.Contracts;

namespace Common.Shipping.Integration
{
    public class OrderShippingResult
    {
        public string ErrorMessage { get; private set; }
        public string SuccessMessage { get; private set; }
        public bool Sucsess { get; private set; }
        public bool Failed { get; private set; }
        public string StatusCode { get; private set; }
        public bool Redirect { get; set; }
        public bool Rejected { get; set; }
        public OrderShipping OrderShipping { get; set; }

        public OrderShippingResult RequestFailed(string errorMessage, string statusCode)
        {
            ErrorMessage = errorMessage;
            Failed = true;
            StatusCode = statusCode;
            return this;
        }

        public void RequestPassed(string successMessage)
        {
            Sucsess = true;
            SuccessMessage = successMessage;
        }

        public OrderShippingResult ForceFail(string error)
        {
            throw new FormatException(error);
        }
    }
}