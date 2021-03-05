using Newtonsoft.Json;
using NServiceBus.Logging;
using Shipping.Integration.Contracts;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Shipping.Integration
{
    public class ApiClient
    {
        static HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        static ILog log = LogManager.GetLogger<ApiClient>();
        private string url;

        public ApiClient(string url)
        {
            this.url = url;
        }

        public async Task<OrderShippingResult> PostShipOrder(OrderShipping orderShipping)
        {
            var apiResult = new OrderShippingResult();
            string statusCode = string.Empty;

            string json = JsonConvert.SerializeObject(orderShipping);

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                using (var response = await httpClient.PostAsync(url + "/OrderShipping/", stringContent).ConfigureAwait(false))
                {
                    statusCode = response.StatusCode.ToString();
                }

                var error = $"Failed to contact '{url}'. HttpStatusCode: {statusCode}";
                log.Info(error);

                apiResult.RequestFailed(error, statusCode);

                return apiResult;
            }
            catch (Exception exception)
            {
                var error = $"Failed to contact '{url}'. Error: {exception.Message}";
                log.Info(error);
                apiResult.RequestFailed(error, statusCode);
                return apiResult;
            }
        }

        public async Task<OrderShippingResult> GetShipOrder(string orderId)
        {
            var apiResult = new OrderShippingResult();
            string statusCode = string.Empty;

            string json = JsonConvert.SerializeObject(orderId);

            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                using (var response = await httpClient.PostAsync(url + "/GetByOrderById/", stringContent).ConfigureAwait(false))
                {
                    statusCode = response.StatusCode.ToString();
                }

                var error = $"Failed to contact '{url}'. HttpStatusCode: {statusCode}";
                log.Info(error);

                apiResult.RequestFailed(error, statusCode);

                return apiResult;
            }
            catch (Exception exception)
            {
                var error = $"Failed to contact '{url}'. Error: {exception.Message}";
                log.Info(error);
                apiResult.RequestFailed(error, statusCode);
                return apiResult;
            }
        }
    }
}