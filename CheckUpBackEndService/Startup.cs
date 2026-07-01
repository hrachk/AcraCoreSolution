using System.Collections.Generic;
using System;
using AcraData.Data;
using AcraUtils;
using Easy.Logger;
using Easy.Logger.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Hosting;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using CheckUpBackEndService;

namespace CheckUpWebService
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log4NetService.Instance.Configure(new System.IO.FileInfo(Configuration["Logging:ConfigPath"]));
            services.AddSingleton<ILogService>(Log4NetService.Instance);

            services.AddScoped<Logger>();

            var cryptor = new Cryptor();

            /*MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
            conn_string.Server = "10.10.2.220";
            conn_string.UserID = "Acwb3usrA";
            conn_string.Password = "AcwWwrA3";
            conn_string.Database = "ACRA3";
            conn_string.PersistSecurityInfo = true;
            conn_string.ConnectionTimeout = 180;
            conn_string.ConvertZeroDateTime = true;
            conn_string.TreatTinyAsBoolean = false;

            services.AddDbContext<Acra3DbContext>(options =>
             options.UseMySql(conn_string.ToString()));*/

            services.AddDbContext<Acra3DbContext>(options =>
                    options.UseMySql(cryptor.DecryptDES(Configuration.GetConnectionString("Acra3Connection")), ServerVersion.AutoDetect(cryptor.DecryptDES(Configuration.GetConnectionString("Acra3Connection")))));
            services.AddOptions();


            //List<string> destinationTo = Configuration.GetSection("FileDestination:Destinations").Get<List<string>>();

            services.AddSingleton(serviceProvider =>
            {
                serviceProvider = services.BuildServiceProvider();
                return new CheckUpService.CheckUpBackService(
                   serviceProvider.GetRequiredService<DbContextOptions<Acra3DbContext>>(),
                   serviceProvider.GetRequiredService<Logger>()
                    );
            });


            // ── Elasticsearch ──────────────────────────────────────────────
            services.AddSingleton<ElasticsearchClient>(sp =>
            {
                var cfg      = sp.GetRequiredService<IConfiguration>();
                var url      = cfg["ElasticSearch:Uri"];
                var username = cfg["ElasticSearch:Username"];
                var password = cfg["ElasticSearch:Password"];

                if (string.IsNullOrWhiteSpace(url))
                    throw new Exception("ElasticSearch:Uri is missing in appsettings.json");

                var settings = new ElasticsearchClientSettings(new Uri(url))
                    .DefaultIndex($"checkup-journal-{DateTime.UtcNow:yyyy.MM.dd}")
                    .Authentication(new BasicAuthentication(username, password))
                    .ServerCertificateValidationCallback((_, _, _, _) => true)
                    .EnableDebugMode();

                return new ElasticsearchClient(settings);
            });

            services.AddScoped<ElasticJournalService>();
            // ──────────────────────────────────────────────────────────────

            services.AddRazorPages();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("Back", "Back", "back/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
