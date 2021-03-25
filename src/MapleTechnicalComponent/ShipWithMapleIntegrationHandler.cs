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
    class ShipWithMapleIntegrationHandler : IHandleMessages<ShipWithMapleIntegration>
    {
        static readonly ILog log = LogManager.GetLogger<ShipWithMapleIntegrationHandler>();

        public async Task Handle(ShipWithMapleIntegration message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMapleIntegrationHandler: Order [{message.OrderId}].");

            MapleApiClient apiClient = new MapleApiClient();
            OrderShipping orderShipping = new OrderShipping() { OrderId = message.OrderId, State = "Posted" };
            OrderShippingResult result = await apiClient
                .PlaceShippingForOrder(orderShipping)
                .ConfigureAwait(false);

            // TODO: expand on that
            if (result.Sucsess)
            {
                await context.Reply(new MapleApiSucsess()
                {
                    OrderId = message.OrderId,
                    ResultMessage = result.Message,
                    TrackingNumber = result.OrderShipping.TrackingNumber
                });
            }

            if (result.Failed)
            {
                await context.Reply(new MapleApiFailureUnknown()
                {
                    OrderId = message.OrderId,
                    ResultMessage = result.Message
                });
            }

            if (result.Rejected)
            {
                await context.Reply(new MapleApiFailureRejection()
                {
                    OrderId = message.OrderId,
                    ResultMessage = result.Message
                });
            }

            if (result.Redirect)
            {
                await context.Reply(new MapleApiFailureRedirect()
                {
                    OrderId = message.OrderId,
                    ResultMessage = result.Message
                });
            }

            log.Info($"ShipWithMapleIntegrationHandler: PlaceShippingForOrder, OrderId: [{message.OrderId}], Result: [{result.StatusCode}, {result.Message}]");
        }
    }
}