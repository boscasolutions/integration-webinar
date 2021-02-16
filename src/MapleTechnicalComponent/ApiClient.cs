using NServiceBus.Logging;
using Shipping.Integration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace MapleTechnicalComponent
{
    public class ApiClient
    {
        const string url = "http://localhost:57811";
        static HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        static ILog log = LogManager.GetLogger<ApiClient>();

        public async Task<ApiResult> CallApi()
        {
            var apiResult = new ApiResult();
            try
            {
                using (var response = await client.GetAsync(url)
                    .ConfigureAwait(false))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        var passMessage = string.Format($"Succeeded in contacting {url}");
                        log.Info(passMessage);
                        
                        apiResult.RequestPassed(passMessage);
                        return apiResult;
                    }

                    var error = $"Failed to contact '{url}'. HttpStatusCode: {response.StatusCode}";
                    log.Info(error);

                    apiResult.RequestFailed(error);

                    return apiResult;
                }
            }
            catch (Exception exception)
            {
                var error = $"Failed to contact '{url}'. Error: {exception.Message}";
                log.Info(error);

                apiResult.RequestFailed(error);

                return apiResult;
            }
        }
    }
}
