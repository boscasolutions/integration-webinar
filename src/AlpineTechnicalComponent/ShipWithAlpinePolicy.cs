using Messages.Commands;
using NServiceBus;
using NServiceBus.Logging;
using System.Threading.Tasks;

namespace MapleTechnicalComponent
{
    #region ShipWithAlpinePolicy
    // Maple is the expansive but reliable and idempotent service 
    class ShipWithAlpinePolicy : Saga<ShipWithAlpinePolicyData>, 
        IAmStartedByMessages<ShipWithAlpine>
    {
        static ILog log = LogManager.GetLogger<ShipWithAlpinePolicy>();

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<ShipWithAlpinePolicyData> mapper)
        {
            mapper.ConfigureMapping<ShipWithAlpine>(message => message.OrderId).ToSaga(saga => saga.OrderId);
        }

        public async Task Handle(ShipWithAlpine message, IMessageHandlerContext context)
        {
            log.Info($"ShipWithMapleHandler: Delaying Order [{message.OrderId}]");

            await context.Send(new ShipWithAlpineIntegration() { OrderId = Data.OrderId });
        }
    }

    internal class ShipWithAlpinePolicyData : ContainSagaData
    {
        public string OrderId { get; internal set; }
    }

    #endregion
}