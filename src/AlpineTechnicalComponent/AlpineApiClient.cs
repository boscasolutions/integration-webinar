using System.Threading.Tasks;
using Common.Shipping.Integration;
using Shipping.Integration.Contracts;

namespace AlpineTechnicalComponent
{
    public class AlpineApiClient
    {
        const string url = "http://localhost:57810";

        public async Task<OrderShippingResult> PlaceShippingForOrder(string orderId)
        {
            var orderShipping = new OrderShipping() { OrderId = orderId, State = "Posted" };

            OrderShippingResult result = await new ApiClient(url).PostShipOrder(orderShipping).ConfigureAwait(false);

            return result;
        }

        internal async Task<OrderShippingResult> GetOrderShippingStatus(string orderId)
        {
            OrderShippingResult result = await new ApiClient(url).GetShipOrder(orderId).ConfigureAwait(false);

            return result;
        }
    }
}