using System;
using Messages.Commands;
using NServiceBus;

namespace AlpineTechnicalComponent
{
    public class EndpointMappings
    {
        internal static Action<TransportExtensions<LearningTransport>> MessageEndpointMappings()
        {
            return transport =>
            {
                RoutingSettings<LearningTransport> routing = transport.Routing();
                routing.RouteToEndpoint(typeof(ShipWithAlpineIntegration), "AlpineTechnicalComponent");
            };
        }
    }
}