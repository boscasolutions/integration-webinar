using Messages.Commands;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace MapleTechnicalComponent
{
    #region ShipWithMaplePolicy
    // Maple is the expansive but reliable and idempotent service 
    class ShipWithMaplePolicy : Saga<ShipWithMaplePolicyData>, 
        IAmStartedByMessages<ShipWithMaple>,
        IHandleMessages<MapleApiSucsess>,
        IHandleMessages<MapleApiFailureUnknown>,
        IHandleMessages<MapleApiFailureRejection>,
        IHandleMessages<MapleApiFailureRedirect>
        {
        static ILog log = LogManager.GetLogger<ShipWithMapleHandler>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShipWithMaplePolicyData> mapper)
        {
            mapper.ConfigureMapping<ShipWithMaple>(message => message.OrderId).ToSaga(saga => saga.OrderId);
        }

        public async Task Handle(ShipWithMaple message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMapleHandler: Delaying Order [{message.OrderId}]");

            await context.Send(new ShipWithMapleIntegration() { OrderId = Data.OrderId });
        }

        public async Task Handle(MapleApiFailureUnknown message, IMessageHandlerContext context)
        {
            // TODO: retry
            await context.Reply(new MapleShipmentFailed());
        }

        public async Task Handle(MapleApiSucsess message, IMessageHandlerContext context)
        {
            await context.Reply(new MapleShipmentAccepted() { TrackingNumber = message.TrackingNumber});
        }

        public async Task Handle(MapleApiFailureRejection message, IMessageHandlerContext context)
        {
            await context.Reply(new MapleShipmentFailed());
        }

        public async Task Handle(MapleApiFailureRedirect message, IMessageHandlerContext context)
        {
            await context.Reply(new MapleShipmentFailed());
        }
    }

    internal class ShipWithMaplePolicyData : ContainSagaData
    {
        public string OrderId { get; internal set; }
    }

    #endregion
}