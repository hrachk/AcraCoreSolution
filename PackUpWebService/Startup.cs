using AcraUtils;
using AutoMapper.Data;
using CheckUpWebService.IdentityModels;
using Easy.Logger;
using Easy.Logger.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using System.Reflection;

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
                .AddJsonFile("appsettings_CICD.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log4NetService.Instance.Configure(new System.IO.FileInfo(Configuration["Logging:ConfigPath"]));
            services.AddSingleton<ILogService>(Log4NetService.Instance);

            services.AddScoped<Logger>();

            //List<string> destinationTo = Configuration.GetSection("FileDestination:Destinations").Get<List<string>>();

            services.Configure<AcraUtils.Configuration.PackUpConfig>(Configuration.GetSection("FileDestination"));
            services.Configure<AcraUtils.Configuration.PackUpConfig>(Configuration.GetSection("CheckUpService"));
            services.Configure<AcraUtils.Configuration.PackUpConfig>(Configuration.GetSection("UploadSwitch"));
            services.Configure<AcraUtils.Configuration.PackUpConfig>(Configuration.GetSection("Time"));
            services.Configure<AcraUtils.Configuration.PackUpConfig>(Configuration.GetSection("VersionControl"));

            services.AddSingleton(serviceProvider =>
            {
                serviceProvider = services.BuildServiceProvider();
                return new CheckUpService.CheckUpService(
                   serviceProvider.GetRequiredService<Logger>(),
                   serviceProvider.GetRequiredService<IOptions<AcraUtils.Configuration.PackUpConfig>>()
                    );
            });


            services.AddScoped<AcraIdentityValidator>();
           

            services.AddAuthentication("Bearer")
               .AddIdentityServerAuthentication(options =>
               {
                   options.Authority = Configuration.GetSection("Identity:IdentityServerUrl").Value;
                   options.RequireHttpsMetadata = false;
                   options.ApiName = "CheckUp";
               });

            services.AddAutoMapper(cfg => cfg.AddDataReaderMapping());
            services.AddRazorPages();

            IdentityModelEventSource.ShowPII = true;

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
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("Back", "Back", "back/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
