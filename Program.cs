using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace hellodotnetcore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            Console.WriteLine("in startup main");
            var userId = Environment.GetEnvironmentVariable("HTTP_X_MS_CLIENT_PRINCIPAL_ID");
            Console.WriteLine("user id env variable: " + userId);

            // userId = httpRequest.Headers["X-MS-CLIENT-PRINCIPAL-ID"].ToString();
            // Console.WriteLine("user id request header: " + userId);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
