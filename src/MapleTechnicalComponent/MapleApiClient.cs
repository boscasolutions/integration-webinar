using System;
using System.Threading.Tasks;
using Common.Shipping.Integration;

namespace MapleTechnicalComponent
{
    public class MapleApiClient
    {
        const string url = "http://localhost:57811";

        public async Task<ApiResult> PlaceShippingForOrder(string orderId)
        {
            string requestString = url + @"/?shiporder=" + orderId;

            ApiResult result = await new ApiClient(requestString).CallApi().ConfigureAwait(false);

            return result;
        }

        internal async Task<ApiResult> GetOrderShippingStatus(string orderId)
        {
            string requestString = url + @"/?shiporderstatus=" + orderId;

            ApiResult result = await new ApiClient(requestString).CallApi().ConfigureAwait(false);

            return result;
        }
    }
}
