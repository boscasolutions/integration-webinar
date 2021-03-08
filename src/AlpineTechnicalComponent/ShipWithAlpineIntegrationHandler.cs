using System.Threading.Tasks;
using Common.Shipping.Integration;
using Messages.Commands;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;

namespace AlpineTechnicalComponent
{
    // Alpine is the cheep not so reliable service
    class ShipWithAlpineIntegrationHandler : IHandleMessages<ShipWithAlpineIntegration>
    {
        static readonly ILog log = LogManager.GetLogger<ShipWithAlpineIntegrationHandler>();

        public async Task Handle(ShipWithAlpineIntegration message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpineIntegrationHandler: Order [{message.OrderId}]");

            AlpineApiClient apiClient = new AlpineApiClient();

            OrderShippingResult result = await apiClient.PlaceShippingForOrder(message.OrderId).ConfigureAwait(false);

            // TODO: expand on that
            if (result.Sucsess)
            {
                await context.Reply(new AlpineApiSucsess() { OrderId = message.OrderId, ResultMessage = result.SuccessMessage, TrackingNumber = result.OrderShipping.TrackingNumber });
            }

            if (result.Failed)
            {
                await context.Reply(new AlpineApiFailureUnknown() { OrderId = message.OrderId, ResultMessage = result.ErrorMessage });
            }

            if (result.Rejected)
            {
                await context.Reply(new AlpineApiFailureRejection() { OrderId = message.OrderId, ResultMessage = result.ErrorMessage });
            }

            if (result.Redirect)
            {
                await context.Reply(new AlpineApiFailureRedirect() { OrderId = message.OrderId, ResultMessage = result.ErrorMessage });
            }

            log.Info($"ShipWithAlpineIntegrationHandler: PlaceShippingForOrder, OrderId: [{message.OrderId}], Result: [{result.StatusCode}, {result.ErrorMessage}]");
        }
    }
}