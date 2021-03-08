using System.Threading.Tasks;
using Messages.Commands;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace Shipping
{
    class OrderPlacedHandler : IHandleMessages<OrderPlaced>
    {
        static readonly ILog log = LogManager.GetLogger<OrderPlacedHandler>();

        public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
        {
            log.Info($"OrderPlaced message received.");

            await context.Send(new ShipOrder() { OrderId = message.OrderId });
        }
    }
}