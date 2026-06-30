using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace EkengWebService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = "PekBackService";
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        var hostConfig = context.Configuration.GetSection("HostOptions");

                        bool useHttps = bool.Parse(hostConfig["UseHttps"]);
                        IPAddress ipAddress = IPAddress.Parse(hostConfig["IPv4"]);
                        int port = int.Parse(hostConfig["Port"]);

                        options.Listen(ipAddress, port);
                    });
                });
    }
}
