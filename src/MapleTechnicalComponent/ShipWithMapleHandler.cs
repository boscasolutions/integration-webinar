using Messages.Commands;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;
using Common.Shipping.Integration;
using System;
using System.Threading.Tasks;

namespace MapleTechnicalComponent
{
    #region ShipWithMapleHandler
    // Maple is the expansive but reliable and idempotent service 
    class ShipWithMapleHandler : IHandleMessages<ShipWithMaple>
    {
        static ILog log = LogManager.GetLogger<ShipWithMapleHandler>();

        const int MaximumTimeMapleMightRespond = 60;
        static Random random = new Random();

        public async Task Handle(ShipWithMaple message, IMessageHandlerContext context)
        {
            var waitingTime = random.Next(MaximumTimeMapleMightRespond);

            log.Info($"ShipWithMapleHandler: Delaying Order [{message.OrderId}] {waitingTime} seconds.");

            var apiClient = new ApiClient();

            ApiResult result = await apiClient.CallApi().ConfigureAwait(false);

            if (result.Pass)
                await context.Reply(new ShipmentAcceptedByMaple());
            if (result.Failed)
                await context.Reply(new ShipmentWithMapleFailed());
        }
    }

    #endregion
}