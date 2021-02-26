using System.Threading.Tasks;
using Common.Shipping.Integration;

namespace AlpineTechnicalComponent
{
    public class AlpineApiClient
    {
        const string url = "http://localhost:57810";

        public async Task<ApiResult> PlaceShippingForOrder(string orderId)
        {
            string requestString = url + @"/?shiporder=" + orderId;

            ApiResult result = await new ApiClient(requestString).CallApi().ConfigureAwait(false);

            return result;
        }
    }
}
