using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace hellodotnetcore
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");

                    Console.WriteLine("in startup main");
                    var userId = Environment.GetEnvironmentVariable("HTTP_X_MS_CLIENT_PRINCIPAL_ID");
                    Console.WriteLine("user id env variable: " + userId);

                    userId = context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].ToString();
                    Console.WriteLine("user id request header: " + context.Request.Headers["X-MS-CLIENT-PRINCIPAL-ID"].ToString());
                    Console.WriteLine("user name request header: " + context.Request.Headers["X-MS-CLIENT-PRINCIPAL-NAME"].ToString());
                });
            });
        }
    }
}
