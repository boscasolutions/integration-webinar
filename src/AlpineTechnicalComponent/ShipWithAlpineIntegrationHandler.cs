using Messages.Commands;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;
using Common.Shipping.Integration;
using System.Threading.Tasks;

namespace AlpineTechnicalComponent
{
    #region ShipWithAlpineHandler
    // Alpine is the cheep not so reliable service
    class ShipWithAlpineIntegrationHandler : IHandleMessages<ShipWithAlpineIntegration>
    {
        static ILog log = LogManager.GetLogger<ShipWithAlpineIntegrationHandler>();

        public async Task Handle(ShipWithAlpineIntegration message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpineIntegrationHandler: Order [{message.OrderId}]");

            var apiClient = new AlpineApiClient();

            ApiResult result = await apiClient.PlaceShippingForOrder(message.OrderId).ConfigureAwait(false);

            // TODO: expand on that
            if (result.Sucsess)
                await context.Reply(new AlpineApiSucsess() { OrderId = message.OrderId, ResultMessage = result.SuccessMessage, TrackingNumber = result.TrackingNumber });
            if (result.Failed)
                await context.Reply(new AlpineApiFailureUnknown() { OrderId = message.OrderId, ResultMessage = result.ErrorMessage });
            if (result.Rejected)
                await context.Reply(new AlpineApiFailureRejection() { OrderId = message.OrderId, ResultMessage = result.ErrorMessage });
            if (result.Redirect)
                await context.Reply(new AlpineApiFailureRedirect() { OrderId = message.OrderId, ResultMessage = result.ErrorMessage });

            log.Info($"ShipWithAlpineIntegrationHandler: PlaceShippingForOrder, OrderId: [{message.OrderId}], Result: [{result.StatusCode}, {result.ErrorMessage}]");
        }
    }
    #endregion
}