using Messages.Commands;
using NServiceBus;
using System;

namespace MapleTechnicalComponent
{
    public class EndpointMappings
    {
        internal static Action<TransportExtensions<LearningTransport>> MessageEndpointMappings()
        {
            return transport =>
            {
                var routing = transport.Routing();
                routing.RouteToEndpoint(typeof(ShipWithMapleIntegration), "MapleTechnicalComponent");
                routing.RouteToEndpoint(typeof(GetOrderShippingStatuMaple), "MapleTechnicalComponent");
            };
        }
    }
}