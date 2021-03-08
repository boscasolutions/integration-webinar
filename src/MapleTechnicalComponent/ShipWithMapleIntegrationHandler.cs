﻿using System.Threading.Tasks;
using Common.Shipping.Integration;
using Messages.Commands;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;

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

            OrderShippingResult result = await apiClient.PlaceShippingForOrder(message.OrderId).ConfigureAwait(false);

            // TODO: expand on that
            if (result.Sucsess)
            {
                await context.Reply(new MapleApiSucsess() { OrderId = message.OrderId, ResultMessage = result.SuccessMessage, TrackingNumber = result.OrderShipping.TrackingNumber });
            }

            if (result.Failed)
            {
                await context.Reply(new MapleApiFailureUnknown() { OrderId = message.OrderId, ResultMessage = result.ErrorMessage });
            }

            if (result.Rejected)
            {
                await context.Reply(new MapleApiFailureRejection() { OrderId = message.OrderId, ResultMessage = result.ErrorMessage });
            }

            if (result.Redirect)
            {
                await context.Reply(new MapleApiFailureRedirect() { OrderId = message.OrderId, ResultMessage = result.ErrorMessage });
            }

            log.Info($"ShipWithMapleIntegrationHandler: PlaceShippingForOrder, OrderId: [{message.OrderId}], Result: [{result.StatusCode}, {result.ErrorMessage}]");
        }
    }
}