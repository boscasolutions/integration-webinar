using System;
using Messages.Commands;
using NServiceBus;

namespace Shipping
{
    public class EndpointMappings
    {
        internal static Action<TransportExtensions<LearningTransport>> MessageEndpointMappings()
        {
            return transport =>
            {
                RoutingSettings<LearningTransport> routing = transport.Routing();
                routing.RouteToEndpoint(typeof(ShipOrder), "Shipping");
                routing.RouteToEndpoint(typeof(ShipWithMaple), "MapleTechnicalComponent");
                routing.RouteToEndpoint(typeof(ShipWithAlpine), "AlpineTechnicalComponent");
            };
        }
    }
}