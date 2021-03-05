using Messages.Commands;
using Messages.Events;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace MapleTechnicalComponent
{
    #region ShipWithMapleWorkflow
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
        static ILog log = LogManager.GetLogger<ShipWithMapleWorkflow>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShipWithMapleWorkflowData> mapper)
        {
            mapper.ConfigureMapping<ShipWithMaple>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<MapleApiSucsess>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<MapleApiFailureUnknown>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<MapleApiFailureRejection>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<MapleApiFailureRedirect>(message => message.OrderId).ToSaga(saga => saga.OrderId);

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
            if (Data.RetryCount < 3)
            {
                Data.RetryCount++;
                // check if order was excepted
                await context.Send(new GetOrderShippingStatuMaple() { OrderId = message.OrderId});
            }
            else
            {
                await context.Publish(new MapleShipmentFailed() { OrderId = message.OrderId, ResultMessage = message.ResultMessage });
            }
        }

        public async Task Handle(MapleApiSucsess message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMapleWorkflow: MapleApiSucsess [OrderId: {message.OrderId}, TrackingId: {message.TrackingNumber}]");

            await context.Publish(new MapleShipmentAccepted() { OrderId = message.OrderId, TrackingNumber = message.TrackingNumber});
        }

        public async Task Handle(MapleApiFailureRejection message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMapleWorkflow: MapleApiFailureRejection [OrderId: {message.OrderId}, Error:  {message.ResultMessage}]");

            await context.Publish(new MapleShipmentFailed() { OrderId = message.OrderId, ResultMessage = message.ResultMessage});
        }

        public async Task Handle(MapleApiFailureRedirect message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMapleWorkflow: MapleApiFailureRedirect [OrderId: {message.OrderId}, Error:  {message.ResultMessage}]");

            await context.Publish(new MapleShipmentFailed() { OrderId = message.OrderId, ResultMessage = message.ResultMessage });
        }
    }

    internal class ShipWithMapleWorkflowData : ContainSagaData
    {
        public string OrderId { get; internal set; }
        public int RetryCount { get; internal set; }
    }

    #endregion
}