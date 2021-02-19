using Messages.Commands;
using NServiceBus;
using System;

namespace Shipping
{
    public class EndpointMappings
    {
        internal static Action<TransportExtensions<LearningTransport>> MessageEndpointMappings()
        {
            return transport =>
            {
                var routing = transport.Routing();
                routing.RouteToEndpoint(typeof(ShipOrder), "Shipping");
                routing.RouteToEndpoint(typeof(ShipWithMaple), "MapleTechnicalComponent");
                routing.RouteToEndpoint(typeof(ShipWithAlpine), "AlpineTechnicalComponent");
            };
        }
    }
}