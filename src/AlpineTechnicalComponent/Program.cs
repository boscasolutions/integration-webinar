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

            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();           

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }
    }
}