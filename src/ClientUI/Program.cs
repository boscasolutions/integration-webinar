using Common.Configuration;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;
using System;
using System.Threading.Tasks;

namespace ClientUI
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "ClientUI";

            var endpointConfiguration = new EndpointConfiguration("ClientUI");
            endpointConfiguration.ApplyEndpointConfiguration();

            var endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            await RunLoop(endpointInstance)
                .ConfigureAwait(false);

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        static ILog log = LogManager.GetLogger<Program>();

        static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            var lastOrder = string.Empty;
            var customerID = "Particular";

            while (true)
            {
                log.Info("Press 'P' to place an order, or 'Q' to quit.");
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.P:
                        // Instantiate the process
                        var orderPlaced = new OrderPlaced
                        {
                            CustomerId = customerID,
                            OrderId = Guid.NewGuid().ToString()
                        };

                        // Send the command
                        log.Info($"Publishing OrderPlaced event, OrderId = {orderPlaced.OrderId}");
                        await endpointInstance.Send(orderPlaced)
                            .ConfigureAwait(false);

                        lastOrder = orderPlaced.OrderId; // Store order identifier to cancel if needed.
                        break;

                    case ConsoleKey.Q:
                        return;

                    default:
                        log.Info("Unknown input. Please try again.");
                        break;
                }
            }
        }
    }
}