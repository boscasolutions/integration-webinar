using NServiceBus;
using NServiceBus.Logging;
using Messages;
using System;
using System.Threading.Tasks;
using Shipping.Integration;

namespace AlpineTechnicalComponent
{
    #region ShipWithAlpineHandler
    // Alpine is the cheep not so relible service

    class ShipWithAlpineHandler : IHandleMessages<ShipWithAlpine>
    {
        static ILog log = LogManager.GetLogger<ShipWithAlpineHandler>();

        const int MaximumTimeAlpineMightRespond = 30;
        static Random random = new Random();

        public async Task Handle(ShipWithAlpine message, IMessageHandlerContext context)
        {
            var waitingTime = random.Next(MaximumTimeAlpineMightRespond);

            log.Info($"ShipWithAlpineHandler: Delaying Order [{message.OrderId}] {waitingTime} seconds.");

            var apiClient = new ApiClient();

            ApiResult result = await apiClient.CallApi().ConfigureAwait(false);

            if(result.Pass)
                await context.Reply(new ShipmentAcceptedByAlpine());
            if (result.Failed)
                await context.Reply(new ShipmentWithAlpineFailed());

        }
    }

    #endregion
}