using Messages.Commands;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;
using Common.Shipping.Integration;
using System.Threading.Tasks;

namespace MapleTechnicalComponent
{
    #region GetOrderShippingStatuMapleHandler
    // Maple is the expansive but reliable and idempotent service 
    class GetOrderShippingStatuMapleHandler : IHandleMessages<GetOrderShippingStatuMaple>
    {
        static ILog log = LogManager.GetLogger<GetOrderShippingStatuMapleHandler>();

        public async Task Handle(GetOrderShippingStatuMaple message, IMessageHandlerContext context)
        {
            log.Info($"GetOrderShippingStatuMapleHandler: Order [{message.OrderId}].");

            var apiClient = new MapleApiClient();

            OrderShippingResult result = await apiClient.GetOrderShippingStatus(message.OrderId).ConfigureAwait(false);

            // TODO: expand on that
            if (result.Sucsess && !string.IsNullOrEmpty(result.OrderShipping.TrackingNumber))
            {
                await context.Reply(new MapleApiSucsess() { OrderId = message.OrderId, ResultMessage = result.SuccessMessage, TrackingNumber = result.OrderShipping.TrackingNumber });
            }
            else 
            {
                await context.Reply(new MapleApiFailureUnknown() { OrderId = message.OrderId, ResultMessage = result.ErrorMessage });
            }

        }
    }
    #endregion
}