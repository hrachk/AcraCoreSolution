using AcraData.Data;
using AcraUtils;
using AcraValidatorWebService.Middlewares;
using Easy.Logger;
using Easy.Logger.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;

namespace AcraValidatorWebService
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log4NetService.Instance.Configure(new System.IO.FileInfo(Configuration["Logging:ConfigPath"]));
            services.AddSingleton<ILogService>(Log4NetService.Instance);
            services.AddScoped<Logger>();
            services.AddOptions();
            services.Configure<AcraUtils.Configuration.ValidatorConfig>(Configuration.GetSection("ValidatorConfig"));

            AcraUtils.Cryptor cryptor = new AcraUtils.Cryptor();

            services.AddDbContext<AcraData.Data.Acra4DbContext>(options =>
            options.UseMySql(cryptor.DecryptDES(Configuration.GetConnectionString("Acra4Connection")),
            ServerVersion.AutoDetect(cryptor.DecryptDES(Configuration.GetConnectionString("Acra4Connection")))));

            services.AddDbContext<AcraData.Data.Acra3DbContext>(options =>
            options.UseMySql(cryptor.DecryptDES(Configuration.GetConnectionString("Acra3Connection")),
            ServerVersion.AutoDetect(cryptor.DecryptDES(Configuration.GetConnectionString("Acra3Connection")))));

            services.AddDbContext<AcraData.Data.AcraJournalDbContext>(options =>
            options.UseMySql(cryptor.DecryptDES(Configuration.GetConnectionString("AcraJournalConnection")),
            ServerVersion.AutoDetect(cryptor.DecryptDES(Configuration.GetConnectionString("AcraJournalConnection")))));


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton(serviceProvider =>
            {
                serviceProvider = services.BuildServiceProvider();
                return new AcraIDServices.AcraIdentityValidatorEkengModel(
                     serviceProvider.GetRequiredService<DbContextOptions<Acra3DbContext>>(),
                     serviceProvider.GetRequiredService<DbContextOptions<Acra4DbContext>>(),
                   serviceProvider.GetRequiredService<Logger>(),
                   serviceProvider.GetRequiredService<DbContextOptions<AcraJournalDbContext>>()
                    );
            });

            services.AddSingleton(serviceProvider =>
            {
                serviceProvider = services.BuildServiceProvider();
                return new AcraIDServices.ConverterModel(
                     serviceProvider.GetRequiredService<DbContextOptions<Acra4DbContext>>(),
                   serviceProvider.GetRequiredService<Logger>(),
                   serviceProvider.GetRequiredService<IOptions<AcraUtils.Configuration.ValidatorConfig>>(),
                   serviceProvider.GetRequiredService<DbContextOptions<Acra3DbContext>>(),
                   serviceProvider.GetRequiredService<DbContextOptions<AcraJournalDbContext>>()
                    );
            });

            services.AddScoped(serviceProvider =>
            {
                serviceProvider = services.BuildServiceProvider();
                return new AcraIDServices.AcraIdentityValidatorBankIDModel(
                     serviceProvider.GetRequiredService<DbContextOptions<Acra3DbContext>>(),
                     serviceProvider.GetRequiredService<DbContextOptions<Acra4DbContext>>(),
                   serviceProvider.GetRequiredService<Logger>()
                    );
            });



            services.AddSingleton(serviceProvider =>
            {
                serviceProvider = services.BuildServiceProvider();
                return new AcraIDServices.AcraIdentityValidatorService((AcraIDServices.AcraIdentityValidatorService.ValidatorType)Enum.Parse(typeof(AcraIDServices.AcraIdentityValidatorService.ValidatorType), Configuration["ValidatorType"]), serviceProvider.GetRequiredService<DbContextOptions<Acra3DbContext>>(),
                    serviceProvider.GetRequiredService<DbContextOptions<AcraJournalDbContext>>(),
                    serviceProvider.GetRequiredService<DbContextOptions<Acra4DbContext>>(),
                   serviceProvider.GetRequiredService<Logger>());
            });

            services.AddScoped(serviceProvider =>
            {
                serviceProvider = services.BuildServiceProvider();
                return new AcraIDServices.CollectAVVInfoService(serviceProvider.GetRequiredService<DbContextOptions<Acra3DbContext>>(),
                    serviceProvider.GetRequiredService<DbContextOptions<AcraJournalDbContext>>(),
                    serviceProvider.GetRequiredService<DbContextOptions<Acra4DbContext>>(),
                   serviceProvider.GetRequiredService<Logger>());
            });


            services.Configure<Token>(Configuration.GetSection("Token"));
            services.AddAutoMapper((Action<AutoMapper.IMapperConfigurationExpression>)null, AppDomain.CurrentDomain.GetAssemblies());
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
            app.UseGetInfoBySNNMiddleware();
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
