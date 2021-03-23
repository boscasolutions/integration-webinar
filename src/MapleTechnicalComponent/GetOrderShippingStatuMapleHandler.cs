using System.Threading.Tasks;
using Common.Shipping.Integration;
using Messages.Commands;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;
using Shipping.Integration.Contracts;

namespace MapleTechnicalComponent
{
    // Maple is the expansive but reliable and idempotent service 
    class GetOrderShippingStatuMapleHandler : IHandleMessages<GetOrderShippingStatuMaple>
    {
        static readonly ILog log = LogManager.GetLogger<GetOrderShippingStatuMapleHandler>();

        public async Task Handle(GetOrderShippingStatuMaple message, IMessageHandlerContext context)
        {
            log.Info($"GetOrderShippingStatuMapleHandler: Order [{message.OrderId}].");

            MapleApiClient apiClient = new MapleApiClient();

            OrderShipping orderShipping = new OrderShipping() { OrderId = message.OrderId, State = "Posted" };
            OrderShippingResult result = await apiClient.GetOrderShippingStatus(orderShipping).ConfigureAwait(false);

            // TODO: expand on that
            if (result.Sucsess && result.OrderShipping != null)
            {
                await context.Reply(new MapleApiSucsess()
                { 
                    OrderId = message.OrderId, 
                    ResultMessage = result.Message, 
                    TrackingNumber = result.OrderShipping.TrackingNumber
                });
            }
            else
            {
                await context.Reply(new MapleApiFailureUnknown() 
                { 
                    OrderId = message.OrderId, 
                    ResultMessage = result.Message
                });
            }
        }
    }
}