using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace TestApi
{
    class Program
    {
        static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:57811/")
                .Build()
                .Run();
        }
    }
}