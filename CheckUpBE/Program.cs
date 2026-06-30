using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CheckUpBE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            .UseKestrel(options =>
            {
                var configs = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json").Build();
                var hostConfig = configs.GetSection("HostOptions");
                bool useHttps;
                IPAddress ipAddress;
                int port;
                if (!Boolean.TryParse(hostConfig["UseHttps"], out useHttps))
                {
                    Console.WriteLine("Invalid setting value for HostOptions/UseHttps");
                    throw new FormatException("Input string was not in the correct format");
                }
                if (!IPAddress.TryParse(hostConfig["IPv4"], out ipAddress))
                {
                    Console.WriteLine("Invalid setting value for HostOptions/IPv4");
                    throw new FormatException("Input string was not in the correct format");
                }
                if (!Int32.TryParse(hostConfig["Port"], out port))
                {
                    Console.WriteLine("Invalid setting value for HostOptions/Port");
                    throw new FormatException("Input string was not in the correct format");
                }
                options.Listen(new IPEndPoint(ipAddress, port));

            }).Build();
    }
}
