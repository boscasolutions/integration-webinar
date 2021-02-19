using Common.Configuration;
using NServiceBus;
using System;
using System.Threading.Tasks;

namespace AlpineTechnicalComponent
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "AlpineTechnicalComponent";

            var endpointConfiguration = new EndpointConfiguration("AlpineTechnicalComponent");
            endpointConfiguration.ApplyEndpointConfiguration();

           var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}