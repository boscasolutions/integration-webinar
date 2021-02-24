using Messages.Commands;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;
using Common.Shipping.Integration;
using System;
using System.Threading.Tasks;

namespace AlpineTechnicalComponent
{
    #region ShipWithAlpineHandler
    // Alpine is the cheep not so relible service

    class ShipWithAlpineHandler : IHandleMessages<ShipWithAlpineIntegration>
    {
        static ILog log = LogManager.GetLogger<ShipWithAlpineHandler>();

        const int MaximumTimeAlpineMightRespond = 30;
        static Random random = new Random();

        public async Task Handle(ShipWithAlpineIntegration message, IMessageHandlerContext context)
        {
            var waitingTime = random.Next(MaximumTimeAlpineMightRespond);

            log.Info($"ShipWithAlpineHandler: Delaying Order [{message.OrderId}] {waitingTime} seconds.");

            var apiClient = new ApiClient();

            ApiResult result = await apiClient.CallApi().ConfigureAwait(false);

            // TODO: expand on that
            if (result.Sucsess)
                await context.Reply(new AlpineApiSucsess());
            if (result.Failed)
                await context.Reply(new AlpineApiFailureUnknown());
            if (result.Redirect)
                await context.Reply(new AlpineApiFailureRejection());
            if (result.Redirect)
                await context.Reply(new AlpineApiFailureRedirect());
        }
    }

    #endregion
}