﻿using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace MapleWebApi
{
    class Program
    {
        internal static int responseSet = 200;
        internal static string Url = "http://localhost:57811/";

        static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseUrls(Url)
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
            Console.WriteLine("\r\nservice URL: {0}", Url);
            Console.WriteLine("\r\nCurrently returning {0}", responseSet.ToString());
        }
    }
}