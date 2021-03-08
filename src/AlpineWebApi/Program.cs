using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace AlpineWebApi
{
    class Program
    {
        internal static int responseSet = 200;

        static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:57810/")
                .Build()
                .Start();

            while (true)
            {
                ReportStatus();

                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine();

                if (key.Key == ConsoleKey.D2)
                {
                    responseSet = 200;
                }

                if (key.Key == ConsoleKey.D3)
                {
                    responseSet = 300;
                }

                if (key.Key == ConsoleKey.D4)
                {
                    responseSet = 400;
                }

                if (key.Key == ConsoleKey.D5)
                {
                    responseSet = 500;
                }
            }
        }

        static void ReportStatus()
        {
            Console.WriteLine("\r\nCurrently returning {0}", responseSet.ToString());
        }
    }
}