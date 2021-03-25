using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Common.Shipping.Integration;
using NServiceBus.Logging;
using Shipping.Integration.Contracts;

namespace AlpineTechnicalComponent
{
    public class AlpineApiClient
    {
        const string url = "http://localhost:57810";
        static readonly HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        static readonly ILog log = LogManager.GetLogger<AlpineApiClient>();

        public async Task<OrderShippingResult> PlaceShippingForOrder(OrderShipping orderShipping)
        {
            OrderShippingResult apiResult = new OrderShippingResult();
            string statusCode = string.Empty;

            try
            {
                using (HttpResponseMessage response = await httpClient
                    .PostAsJsonAsync(url + "/OrderShipping/", orderShipping)
                    .ConfigureAwait(false))
                {
                    statusCode = response.StatusCode.ToString();

                    response.EnsureSuccessStatusCode();

                    apiResult.OrderShipping = await response.Content
                        .ReadFromJsonAsync<OrderShipping>()
                        .ConfigureAwait(false);
                }

                string info = $"Api: '{url}'/OrderShipping/'{orderShipping.OrderId}'. HttpStatusCode: {statusCode}";

                log.Info(info);

                apiResult.RequestPassed(info);

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

        public async Task<OrderShippingResult> GetOrderShippingStatus(OrderShipping orderShipping)
        {
            OrderShippingResult apiResult = new OrderShippingResult();
            string statusCode = string.Empty;

            try
            {
                using (HttpResponseMessage response = await httpClient
                    .PostAsJsonAsync(url + "/OrderShipping/GetByOrderById/", orderShipping)
                    .ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    apiResult.OrderShipping = await response.Content
                        .ReadFromJsonAsync<OrderShipping>()
                        .ConfigureAwait(false);
                }

                string info = $"Api: '{url}'/OrderShipping/GetByOrderById/'{orderShipping.OrderId}'. HttpStatusCode: {statusCode}";

                log.Info(info);

                apiResult.RequestPassed(info);

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