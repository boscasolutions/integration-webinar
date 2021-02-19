using Messages.Commands;
using NServiceBus;
using System;

namespace ClientUI
{
    public class EndpointMappings
    {
        internal static Action<TransportExtensions<LearningTransport>> MessageEndpointMappings()
        {
            return transport =>
            {
                var routing = transport.Routing();
                routing.RouteToEndpoint(typeof(PlaceOrder), "Sales");
                routing.RouteToEndpoint(typeof(CancelOrder), "Sales");

            };
        }
    }
}