using Messages;
using Messages.Commands;
using Messages.Events;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace AlpineTechnicalComponent
{
    #region ShipWithAlpinePolicy
    // Alpine is the expansive but reliable and idempotent service 
    class ShipWithAlpinePolicy : Saga<ShipWithAlpinePolicyData>, 
        IAmStartedByMessages<ShipWithAlpine>,
        IHandleMessages<AlpineApiSucsess>,
        IHandleMessages<AlpineApiFailureUnknown>,
        IHandleMessages<AlpineApiFailureRejection>,
        IHandleMessages<AlpineApiFailureRedirect>
    {
        static ILog log = LogManager.GetLogger<ShipWithAlpinePolicy>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShipWithAlpinePolicyData> mapper)
        {
            mapper.ConfigureMapping<ShipWithAlpine>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<AlpineApiSucsess>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<AlpineApiFailureUnknown>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<AlpineApiFailureRejection>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<AlpineApiFailureRedirect>(message => message.OrderId).ToSaga(saga => saga.OrderId);
        }

        public async Task Handle(ShipWithAlpine message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpinePolicy: Delaying Order [{message.OrderId}]");

            await context.Send(new ShipWithAlpineIntegration() { OrderId = Data.OrderId });
        }

        public async Task Handle(AlpineApiSucsess message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpinePolicy: AlpineApiSucsess [OrderId: {Data.OrderId}, Tracking: {message.TrackingNumber}]");

            await context.Publish(new AlpineShipmentAccepted() { TrackingNumber = message.TrackingNumber, OrderId = Data.OrderId });
        }

        public async Task Handle(AlpineApiFailureUnknown message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpinePolicy: AlpineApiFailureUnknown [OrderId: {Data.OrderId}, Error:  {message.ResultMessage}]");

            // TODO: retry?
            await context.Publish(new AlpineShipmentFailed() { OrderId = Data.OrderId, ResultMessage = message.ResultMessage });
        }

        public async Task Handle(AlpineApiFailureRejection message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpinePolicy: AlpineApiFailureRejection [OrderId: {Data.OrderId}, Error:  {message.ResultMessage}]");

            await context.Publish(new AlpineShipmentFailed() { OrderId = Data.OrderId, ResultMessage = message.ResultMessage });
        }

        public async Task Handle(AlpineApiFailureRedirect message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpinePolicy: AlpineApiFailureRedirect [OrderId: {Data.OrderId}, Error:  {message.ResultMessage}]");

            await context.Publish(new AlpineShipmentFailed() { OrderId = Data.OrderId, ResultMessage = message.ResultMessage });
        }
    }

    internal class ShipWithAlpinePolicyData : ContainSagaData
    {
        public string OrderId { get; internal set; }
    }

    #endregion
}