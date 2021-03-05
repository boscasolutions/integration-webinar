using Messages.Commands;
using Messages.Events;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace AlpineTechnicalComponent
{
    #region ShipWithAlpineWorkflow
    // Alpine is the expansive but reliable and idempotent service 
    class ShipWithAlpineWorkflow : Saga<ShipWithAlpineWorkflowData>, 
        IAmStartedByMessages<ShipWithAlpine>,
        IHandleMessages<AlpineApiSucsess>,
        IHandleMessages<AlpineApiFailureUnknown>,
        IHandleMessages<AlpineApiFailureRejection>,
        IHandleMessages<AlpineApiFailureRedirect>
    {
        static ILog log = LogManager.GetLogger<ShipWithAlpineWorkflow>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShipWithAlpineWorkflowData> mapper)
        {
            mapper.ConfigureMapping<ShipWithAlpine>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<AlpineApiSucsess>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<AlpineApiFailureUnknown>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<AlpineApiFailureRejection>(message => message.OrderId).ToSaga(saga => saga.OrderId);
            mapper.ConfigureMapping<AlpineApiFailureRedirect>(message => message.OrderId).ToSaga(saga => saga.OrderId);
        }

        public async Task Handle(ShipWithAlpine message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpineWorkflow: Shipping Order [{message.OrderId}]");

            await context.Send(new ShipWithAlpineIntegration() { OrderId = message.OrderId });
        }

        public async Task Handle(AlpineApiSucsess message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpineWorkflow: AlpineApiSucsess [OrderId: {message.OrderId}, Tracking: {message.TrackingNumber}]");

            await context.Publish(new AlpineShipmentAccepted() { OrderId = message.OrderId, TrackingNumber = message.TrackingNumber});
        }

        public async Task Handle(AlpineApiFailureUnknown message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpineWorkflow: AlpineApiFailureUnknown [OrderId: {message.OrderId}, Error:  {message.ResultMessage}]");

            // TODO: retry?
            await context.Publish(new AlpineShipmentFailed() { OrderId = message.OrderId, ResultMessage = message.ResultMessage });
        }

        public async Task Handle(AlpineApiFailureRejection message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpineWorkflow: AlpineApiFailureRejection [OrderId: {message.OrderId}, Error:  {message.ResultMessage}]");

            await context.Publish(new AlpineShipmentFailed() { OrderId = message.OrderId, ResultMessage = message.ResultMessage });
        }

        public async Task Handle(AlpineApiFailureRedirect message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithAlpineWorkflow: AlpineApiFailureRedirect [OrderId: {message.OrderId}, Error:  {message.ResultMessage}]");

            await context.Publish(new AlpineShipmentFailed() { OrderId = message.OrderId, ResultMessage = message.ResultMessage });
        }
    }

    internal class ShipWithAlpineWorkflowData : ContainSagaData
    {
        public string OrderId { get; internal set; }
    }
    #endregion
}