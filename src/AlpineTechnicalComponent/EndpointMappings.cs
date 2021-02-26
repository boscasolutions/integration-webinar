using Messages.Commands;
using NServiceBus;
using System;

namespace AlpineTechnicalComponent
{
    public class EndpointMappings
    {
        internal static Action<TransportExtensions<LearningTransport>> MessageEndpointMappings()
        {
            return transport =>
            {
                var routing = transport.Routing();
                routing.RouteToEndpoint(typeof(ShipWithAlpineIntegration), "AlpineTechnicalComponent");
            };
        }
    }
}