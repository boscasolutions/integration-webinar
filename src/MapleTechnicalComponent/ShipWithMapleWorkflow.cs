﻿using System.Threading.Tasks;
using Messages.Commands;
using Messages.Events;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;

namespace MapleTechnicalComponent
{
    // Maple is the inexpensive but less reliable and not idempotent service 
    // That means we need to watch put for:
    // Retries: we need to make sure we don't ship the order twice, if there is a failure 
    // we need to first query the service and see if the order was already accepted
    // we can limit this to the 500 range (timeout, exception)
    class ShipWithMapleWorkflow : Saga<ShipWithMapleWorkflowData>,
        IAmStartedByMessages<ShipWithMaple>,
        IHandleMessages<MapleApiSucsess>,
        IHandleMessages<MapleApiFailureUnknown>,
        IHandleMessages<MapleApiFailureRejection>,
        IHandleMessages<MapleApiFailureRedirect>
    {
        static readonly ILog log = LogManager.GetLogger<ShipWithMapleWorkflow>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShipWithMapleWorkflowData> mapper)
        {
            mapper.MapSaga(saga => saga.OrderId)
                .ToMessage<ShipWithMaple>(message => message.OrderId)
                .ToMessage<MapleApiSucsess>(message => message.OrderId)
                .ToMessage<MapleApiFailureUnknown>(message => message.OrderId)
                .ToMessage<MapleApiFailureRejection>(message => message.OrderId)
                .ToMessage<MapleApiFailureRedirect>(message => message.OrderId);
        }

        public async Task Handle(ShipWithMaple message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMapleWorkflow: ShipWithMaple [{message.OrderId}]");

            await context.Send(new ShipWithMapleIntegration() { OrderId = message.OrderId });
        }

        public async Task Handle(MapleApiFailureUnknown message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMapleWorkflow: MapleApiFailureUnknown [OrderId: {message.OrderId}, Error:  {message.ResultMessage}]");

            // TODO: retry
            if (Data.RetryCount < 2)
            {
                Data.RetryCount++;
                // check if order was excepted
                await context.Send(new GetOrderShippingStatuMaple()
                {
                    OrderId = message.OrderId
                }).ConfigureAwait(false);
            }
            else
            {
                await context.Publish(new MapleShipmentFailed()
                {
                    OrderId = message.OrderId,
                    ResultMessage = message.ResultMessage
                }).ConfigureAwait(false);
            }
        }

        public async Task Handle(MapleApiSucsess message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMapleWorkflow: MapleApiSucsess [OrderId: {message.OrderId}, TrackingId: {message.TrackingNumber}]");

            await context.Publish(new MapleShipmentAccepted() { OrderId = message.OrderId, TrackingNumber = message.TrackingNumber });
        }

        public async Task Handle(MapleApiFailureRejection message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMapleWorkflow: MapleApiFailureRejection [OrderId: {message.OrderId}, Error:  {message.ResultMessage}]");

            await context.Publish(new MapleShipmentFailed() { OrderId = message.OrderId, ResultMessage = message.ResultMessage });
        }

        public async Task Handle(MapleApiFailureRedirect message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMapleWorkflow: MapleApiFailureRedirect [OrderId: {message.OrderId}, Error:  {message.ResultMessage}]");

            await context.Publish(new MapleShipmentFailed() { OrderId = message.OrderId, ResultMessage = message.ResultMessage });
        }
    }

    internal class ShipWithMapleWorkflowData : ContainSagaData
    {
        public string OrderId { get; set; }
        public int RetryCount { get; set; }
    }
}