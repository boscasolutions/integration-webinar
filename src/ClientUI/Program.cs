using System;
using System.Threading.Tasks;
using Common.Configuration;
using Messages.Events;
using NServiceBus;
using NServiceBus.Logging;

namespace ClientUI
{
    class Program
    {
        static async Task Main()
        {
            Console.Title = "ClientUI";

            EndpointConfiguration endpointConfiguration = new EndpointConfiguration("ClientUI");
            endpointConfiguration.ApplyEndpointConfiguration();

            IEndpointInstance endpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            await RunLoop(endpointInstance)
                .ConfigureAwait(false);

            await endpointInstance.Stop()
                .ConfigureAwait(false);
        }

        static readonly ILog log = LogManager.GetLogger<Program>();

        static async Task RunLoop(IEndpointInstance endpointInstance)
        {
            string lastOrder = string.Empty;
            string customerID = "Particular";

            while (true)
            {
                log.Info("Press 'P' to place an order, or 'Q' to quit.");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.P:
                        // Instantiate the process
                        OrderPlaced orderPlaced = new OrderPlaced
                        {
                            CustomerId = customerID,
                            OrderId = Guid.NewGuid().ToString()
                        };

                        // Send the command
                        log.Info($"Publishing OrderPlaced event, OrderId = {orderPlaced.OrderId}");
                        await endpointInstance.Publish(orderPlaced)
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