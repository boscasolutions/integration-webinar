using System;
using Messages.Commands;
using NServiceBus;

namespace MapleTechnicalComponent
{
    public class EndpointMappings
    {
        internal static Action<TransportExtensions<LearningTransport>> MessageEndpointMappings()
        {
            return transport =>
            {
                RoutingSettings<LearningTransport> routing = transport.Routing();
                routing.RouteToEndpoint(typeof(ShipWithMapleIntegration), "MapleTechnicalComponent");
                routing.RouteToEndpoint(typeof(GetOrderShippingStatuMaple), "MapleTechnicalComponent");
            };
        }
    }
}