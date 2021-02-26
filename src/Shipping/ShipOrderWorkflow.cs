namespace Shipping
{
    using Messages.Commands;
    using Messages.Events;
    using Messages.Replys;
    using NServiceBus;
    using NServiceBus.Logging;
    using System;
    using System.Threading.Tasks;

    class ShipOrderWorkflow :
        Saga<ShipOrderWorkflow.ShipOrderData>,
        IAmStartedByMessages<ShipOrder>,
        IHandleMessages<MapleShipmentAccepted>,
        IHandleMessages<AlpineShipmentAccepted>,
        IHandleMessages<MapleShipmentFailed>,
        IHandleMessages<AlpineShipmentFailed>,
        IHandleTimeouts<ShipOrderWorkflow.ShippingEscalation>
    {
        static ILog log = LogManager.GetLogger<ShipOrderWorkflow>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShipOrderData> mapper)
        {
            mapper.ConfigureMapping<ShipOrder>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<MapleShipmentAccepted>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<AlpineShipmentAccepted>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<MapleShipmentFailed>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<AlpineShipmentFailed>(message => message.OrderId).ToSaga(saga => saga.OrderId);
        }

        public async Task Handle(ShipOrder message, IMessageHandlerContext context)
        {
            log.Info($"ShipOrderWorkflow for Order [{Data.OrderId}] - Trying Maple first.");

            // Execute order to ship with Maple
            await context.Send(new ShipWithMaple() { OrderId = Data.OrderId });

            // Add timeout to escalate if Maple did not ship on time.
            await RequestTimeout(context, TimeSpan.FromSeconds(20), new ShippingEscalation());
        }

        public Task Handle(MapleShipmentAccepted message, IMessageHandlerContext context)
        {
            if (!Data.ShipmentOrderSentToAlpine)
            {
                log.Info($"Order [{Data.OrderId}] - Successfully shipped with Maple with TrackingNumber: [{message.TrackingNumber}]");

                Data.ShipmentAcceptedByMaple = true;

                MarkAsComplete();
            }

            return Task.CompletedTask;
        }

        public Task Handle(AlpineShipmentAccepted message, IMessageHandlerContext context)
        {
            log.Info($"Order [{Data.OrderId}] - Successfully shipped with Alpine with TrackingNumber: [{message.TrackingNumber}]");

            Data.ShipmentAcceptedByAlpine = true;

            MarkAsComplete();

            return Task.CompletedTask;
        }

        public async Task Timeout(ShippingEscalation timeout, IMessageHandlerContext context)
        {
            if (!Data.ShipmentAcceptedByMaple)
            {
                if (!Data.ShipmentOrderSentToAlpine)
                {
                    log.Info($"Order [{Data.OrderId}] - No answer from Maple, let's try Alpine.");
                    Data.ShipmentOrderSentToAlpine = true;
                    await context.Send(new ShipWithAlpine() { OrderId = Data.OrderId });
                    await RequestTimeout(context, TimeSpan.FromSeconds(20), new ShippingEscalation());
                }
                else if (!Data.ShipmentAcceptedByAlpine) // No response from Maple nor Alpine
                {
                    log.Warn($"Order [{Data.OrderId}] - No answer from Maple/Alpine. We need to escalate!");

                    // escalate to Warehouse Manager!
                    await context.Publish<ShipmentFailed>();

                    MarkAsComplete();
                }
            }
        }

        public Task Handle(MapleShipmentFailed message, IMessageHandlerContext context)
        {
            // placeholder
            log.Info($"Order [{Data.OrderId}] - Shipment With Maple Failed.");

            return Task.CompletedTask;
        }

        public Task Handle(AlpineShipmentFailed message, IMessageHandlerContext context)
        {
            // placeholder
            log.Info($"Order [{Data.OrderId}] - Shipment With Alpine Failed.");

            return Task.CompletedTask;
        }

        internal class ShipOrderData : ContainSagaData
        {
            public string OrderId { get; set; }
            public bool ShipmentAcceptedByMaple { get; set; }
            public bool ShipmentOrderSentToAlpine { get; set; }
            public bool ShipmentAcceptedByAlpine { get; set; }
        }

        internal class ShippingEscalation
        {
        }
    }
}