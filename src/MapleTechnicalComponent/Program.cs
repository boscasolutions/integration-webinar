using System;
using System.Threading.Tasks;
using Common.Configuration;
using NServiceBus;

namespace MapleTechnicalComponent
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "MapleTechnicalComponent";

            EndpointConfiguration endpointConfiguration = new EndpointConfiguration("MapleTechnicalComponent");
            endpointConfiguration.ApplyEndpointConfiguration(EndpointMappings.MessageEndpointMappings());

            var recoverability = endpointConfiguration.Recoverability();
            recoverability.Immediate(
                immediate =>
                {
                    immediate.NumberOfRetries(0);
                });

            IEndpointInstance endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}