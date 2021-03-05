namespace Shipping
{
    using Messages.Commands;
    using Messages.Events;
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
        IHandleMessages<AlpineShipmentFailed>
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
        }

        public async Task Handle(MapleShipmentAccepted message, IMessageHandlerContext context)
        {
            if (!Data.ShipmentOrderSentToAlpine)
            {
                log.Info($"Order [{Data.OrderId}] - Successfully shipped with Maple with TrackingNumber: [{message.TrackingNumber}]");

                Data.ShipmentAcceptedByMaple = true;
            }
            await ShippingEscalation(context);
        }

        public async Task Handle(AlpineShipmentAccepted message, IMessageHandlerContext context)
        {
            log.Info($"Order [{Data.OrderId}] - Successfully shipped with Alpine with TrackingNumber: [{message.TrackingNumber}]");

            Data.ShipmentAcceptedByAlpine = true;

            await ShippingEscalation(context);
        }

        public async Task Handle(MapleShipmentFailed message, IMessageHandlerContext context)
        {
            // placeholder
            log.Info($"Order [{Data.OrderId}] - Shipment With Maple Failed.");

            await ShippingEscalation(context);
        }

        public async Task Handle(AlpineShipmentFailed message, IMessageHandlerContext context)
        {
            // placeholder
            log.Info($"Order [{Data.OrderId}] - Shipment With Alpine Failed.");

            await ShippingEscalation(context);
        }

        public async Task ShippingEscalation(IMessageHandlerContext context)
        {
            if (!Data.ShipmentAcceptedByMaple)
            {
                if (!Data.ShipmentOrderSentToAlpine)
                {
                    log.Info($"Order [{Data.OrderId}] - No answer from Maple, let's try Alpine.");
                    Data.ShipmentOrderSentToAlpine = true;
                    await context.Send(new ShipWithAlpine() { OrderId = Data.OrderId });
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

        internal class ShipOrderData : ContainSagaData
        {
            public string OrderId { get; set; }
            public bool ShipmentAcceptedByMaple { get; set; }
            public bool ShipmentOrderSentToAlpine { get; set; }
            public bool ShipmentAcceptedByAlpine { get; set; }
        }
    }
}