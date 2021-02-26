using Messages.Commands;
using Messages.Replys;
using NServiceBus;
using NServiceBus.Logging;
using Common.Shipping.Integration;
using System.Threading.Tasks;

namespace MapleTechnicalComponent
{
    #region GetOrderShippingStatuMapleHandler
    // Maple is the expansive but reliable and idempotent service 
    class GetOrderShippingStatuMapleHandler : IHandleMessages<GetOrderShippingStatuMaple>
    {
        static ILog log = LogManager.GetLogger<GetOrderShippingStatuMapleHandler>();

        public async Task Handle(GetOrderShippingStatuMaple message, IMessageHandlerContext context)
        {
            log.Info($"GetOrderShippingStatuMapleHandler: Order [{message.OrderId}].");

            var apiClient = new MapleApiClient();

            ApiResult result = await apiClient.GetOrderShippingStatus(message.OrderId).ConfigureAwait(false);

            // TODO: expand on that
            if (result.Sucsess)
                await context.Reply(new MapleApiSucsess() { ResultMessage = result.SuccessMessage, TrackingNumber = result.TrackingNumber});
            if (result.Failed)
                await context.Reply(new MapleApiFailureUnknown() { ResultMessage = result.ErrorMessage});
            if(result.Rejected)
                await context.Reply(new MapleApiFailureRejection() { ResultMessage = result.ErrorMessage });
            if (result.Redirect)
                await context.Reply(new MapleApiFailureRedirect() { ResultMessage = result.ErrorMessage });
        }
    }
    #endregion
}