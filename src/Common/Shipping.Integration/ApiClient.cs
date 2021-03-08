using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NServiceBus.Logging;
using Shipping.Integration.Contracts;

namespace Common.Shipping.Integration
{
    public class ApiClient
    {
        static readonly HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        static readonly ILog log = LogManager.GetLogger<ApiClient>();
        private readonly string url;

        public ApiClient(string url)
        {
            this.url = url;
        }

        public async Task<OrderShippingResult> PostShipOrder(OrderShipping orderShipping)
        {
            OrderShippingResult apiResult = new OrderShippingResult();
            string statusCode = string.Empty;

            string json = JsonConvert.SerializeObject(orderShipping);

            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(url + "/OrderShipping/", stringContent).ConfigureAwait(false))
                {
                    statusCode = response.StatusCode.ToString();
                }

                string error = $"Failed to contact '{url}'. HttpStatusCode: {statusCode}";
                log.Info(error);

                apiResult.RequestFailed(error, statusCode);

                return apiResult;
            }
            catch (Exception exception)
            {
                string error = $"Failed to contact '{url}'. Error: {exception.Message}";
                log.Info(error);
                apiResult.RequestFailed(error, statusCode);
                return apiResult;
            }
        }

        public async Task<OrderShippingResult> GetShipOrder(string orderId)
        {
            OrderShippingResult apiResult = new OrderShippingResult();
            string statusCode = string.Empty;

            string json = JsonConvert.SerializeObject(orderId);

            StringContent stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            try
            {
                using (HttpResponseMessage response = await httpClient.PostAsync(url + "/GetByOrderById/", stringContent).ConfigureAwait(false))
                {
                    statusCode = response.StatusCode.ToString();
                }

                string error = $"Failed to contact '{url}'. HttpStatusCode: {statusCode}";
                log.Info(error);

                apiResult.RequestFailed(error, statusCode);

                return apiResult;
            }
            catch (Exception exception)
            {
                string error = $"Failed to contact '{url}'. Error: {exception.Message}";
                log.Info(error);
                apiResult.RequestFailed(error, statusCode);
                return apiResult;
            }
        }
    }
}