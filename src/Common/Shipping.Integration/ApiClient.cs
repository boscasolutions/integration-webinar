using NServiceBus.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Common.Shipping.Integration
{
    public class ApiClient
    {
        static HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        static ILog log = LogManager.GetLogger<ApiClient>();
        private string url;

        public ApiClient(string url)
        {
            this.url = url;
        }

        public async Task<ApiResult> CallApi()
        {
            var apiResult = new ApiResult();
            string statusCode = string.Empty;

            try
            {
                using (var response = await client.GetAsync(url)
                    .ConfigureAwait(false))
                {
                    statusCode = response.StatusCode.ToString();

                    if (response.IsSuccessStatusCode)
                    {
                        var passMessage = string.Format($"Succeeded in contacting {url}");
                        log.Info(passMessage);

                        apiResult.RequestPassed(passMessage, string.Empty);

                        return apiResult;
                    }

                    var error = $"Failed to contact '{url}'. HttpStatusCode: {statusCode}";
                    log.Info(error);

                    apiResult.RequestFailed(error, statusCode);

                    return apiResult;
                }
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
