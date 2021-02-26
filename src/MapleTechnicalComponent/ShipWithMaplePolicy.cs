using Messages.Commands;
using Messages.Events;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace MapleTechnicalComponent
{
    #region ShipWithMaplePolicy
    // Maple is the inexpansive but less reliable and not idempotent service 
    // That means we need to watch put for:
    // Retries: we need to make sure we dont ship the order twice, if there is a failiur 
    // we need to first query the service and see if the order was already accepted
    // we can limit this to the 500 range (timeout, exception)
    class ShipWithMaplePolicy : Saga<ShipWithMaplePolicyData>, 
        IAmStartedByMessages<ShipWithMaple>,
        IHandleMessages<MapleApiSucsess>,
        IHandleMessages<MapleApiFailureUnknown>,
        IHandleMessages<MapleApiFailureRejection>,
        IHandleMessages<MapleApiFailureRedirect>
        {
        static ILog log = LogManager.GetLogger<ShipWithMaplePolicy>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShipWithMaplePolicyData> mapper)
        {
            mapper.ConfigureMapping<ShipWithMaple>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<MapleApiSucsess>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<MapleApiFailureUnknown>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<MapleApiFailureRejection>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<MapleApiFailureRedirect>(message => message.OrderId).ToSaga(saga => saga.OrderId);

        }

        public async Task Handle(ShipWithMaple message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMaplePolicy: ShipWithMaple [{message.OrderId}]");

            await context.Send(new ShipWithMapleIntegration() { OrderId = Data.OrderId });
        }

        public async Task Handle(MapleApiFailureUnknown message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMaplePolicy: MapleApiFailureUnknown [OrderId: {Data.OrderId}, Error:  {message.ResultMessage}]");

            // TODO: retry
            await context.Publish(new MapleShipmentFailed() {OrderId = Data.OrderId, ResultMessage = message.ResultMessage });
        }

        public async Task Handle(MapleApiSucsess message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMaplePolicy: MapleApiSucsess [OrderId: {Data.OrderId}, TrackingId: {message.TrackingNumber}]");

            await context.Publish(new MapleShipmentAccepted() { OrderId = Data.OrderId, TrackingNumber = message.TrackingNumber});
        }

        public async Task Handle(MapleApiFailureRejection message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMaplePolicy: MapleApiFailureRejection [OrderId: {Data.OrderId}, Error:  {message.ResultMessage}]");

            await context.Publish(new MapleShipmentFailed() { OrderId = Data.OrderId, ResultMessage = message.ResultMessage});
        }

        public async Task Handle(MapleApiFailureRedirect message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMaplePolicy: MapleApiFailureRedirect [OrderId: {Data.OrderId}, Error:  {message.ResultMessage}]");

            await context.Publish(new MapleShipmentFailed() { OrderId = Data.OrderId, ResultMessage = message.ResultMessage });
        }
    }

    internal class ShipWithMaplePolicyData : ContainSagaData
    {
        public string OrderId { get; internal set; }
    }

    #endregion
}