using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AcraIdentityFE
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
                if (useHttps)
                {
                    options.Listen(new IPEndPoint(ipAddress, port), listenOptions =>
                    {
                        var serverCertSKI = hostConfig["ServerCertificateSKI"];

                        var httpsConnectionAdapterOptions = new HttpsConnectionAdapterOptions()
                        {
                            ClientCertificateMode = ClientCertificateMode.AllowCertificate,
                            SslProtocols = System.Security.Authentication.SslProtocols.Tls,
                            ServerCertificate = GetServerCertificate(serverCertSKI)
                        };
                        listenOptions.UseHttps(httpsConnectionAdapterOptions);
                    });
                }
                else
                {
                    options.Listen(new IPEndPoint(ipAddress, port));
                }

            }).Build();

        public static X509Certificate2 GetServerCertificate(string serverCertSKI)
        {
            X509Store store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            var collection = store.Certificates.Find(X509FindType.FindBySubjectKeyIdentifier, serverCertSKI, true);
            store.Close();
            if (collection.Count > 0)
            {
                return collection[0];
            }

            return null;
        }

    }
}
