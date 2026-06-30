using AcraData.Data;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PekBackService;
using System;

namespace EkengWebService
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
            var db = new AcraJournalDbContext(new DbContextOptionsBuilder<AcraJournalDbContext>()
                .UseMySql(Configuration.GetConnectionString("AcraJournalConnection"), ServerVersion.AutoDetect(Configuration.GetConnectionString("AcraJournalConnection"))).Options);
            Console.WriteLine(db.Database.GetDbConnection().State);
        
        }

        // This method gets called by the runtime. Use this method to add services to the container.

        public void ConfigureServices(IServiceCollection services)
        {
            var cryptor = new AcraUtils.Cryptor();

            var acra3Conn = cryptor.DecryptDES(Configuration.GetConnectionString("Acra3Connection"));
            //var journalConn = cryptor.DecryptDES(Configuration.GetConnectionString("AcraJournalConnection"));
            var journalConn = Configuration.GetConnectionString("AcraJournalConnection");

            services.AddDbContext<AcraData.Data.Acra3DbContext>(options =>
                options.UseMySql(acra3Conn, ServerVersion.AutoDetect(acra3Conn)));

            services.AddDbContext<AcraData.Data.AcraJournalDbContext>(options =>
                options.UseMySql(journalConn, ServerVersion.AutoDetect(journalConn)));



            // ELASTIC
            services.AddSingleton<ElasticsearchClient>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();

                var url = configuration["ElasticSearch:Uri"];
                var defaultIndex = configuration["ElasticSearch:DefaultIndex"];
                var username = configuration["ElasticSearch:Username"];
                var password = configuration["ElasticSearch:Password"];

                if (string.IsNullOrWhiteSpace(url))
                    throw new Exception("ElasticSearch:Uri is missing");
                var settings = new ElasticsearchClientSettings(new Uri(url))
                   .DefaultIndex($"pek-journal-{DateTime.UtcNow.ToString("yyyy.MM.dd")}")
                   .Authentication(new BasicAuthentication(username, password))
                   .ServerCertificateValidationCallback((sender, certificate, chain, errors) => true)
                   .EnableDebugMode();
                //var settings = new ElasticsearchClientSettings(new Uri(url))
                //    .DefaultIndex(defaultIndex);

                //if (!string.IsNullOrEmpty(username))
                //{
                //    settings = settings.Authentication(
                //        new BasicAuthentication(username, password)
                //    );
                //}

                return new ElasticsearchClient(settings);
            });

            services.AddScoped<ElasticService>();
            services.AddScoped<ElasticJournalService>();
            services.AddScoped<PekJournalModel>();

            services.Configure<AcraUtils.Configuration.ValidatorConfig>(
                Configuration.GetSection("ValidatorConfig"));

            services.AddRazorPages();
            services.AddControllers();

            
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionHandler("/Home/Error");

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    "Back",
                    "Back",
                    "back/{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
