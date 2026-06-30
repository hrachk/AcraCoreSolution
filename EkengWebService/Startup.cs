using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcraUtils;
using Easy.Logger;
using Easy.Logger.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

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
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log4NetService.Instance.Configure(new System.IO.FileInfo(Configuration["Logging:ConfigPath"]));
            services.AddSingleton<ILogService>(Log4NetService.Instance);
            services.AddScoped<Logger>();           
            services.AddOptions();
            
            services.Configure<AcraUtils.Configuration.EkengConfig>(Configuration.GetSection("EkengConfiguration"));
            services.Configure<AcraUtils.Configuration.AVVConfig>(Configuration.GetSection("AVVConfiguration"));
            services.AddSingleton(serviceProvider =>
            {
                serviceProvider = services.BuildServiceProvider();
                return new AcraIDServices.EkengClient(                   
                   serviceProvider.GetRequiredService<Logger>(),
                   serviceProvider.GetRequiredService<IOptions<AcraUtils.Configuration.EkengConfig>>()
                    );
            });

            services.AddSingleton(serviceProvider =>
            {
                serviceProvider = services.BuildServiceProvider();
                return new AcraIDServices.AVVClient(
                   serviceProvider.GetRequiredService<Logger>(),
                   serviceProvider.GetRequiredService<IOptions<AcraUtils.Configuration.AVVConfig>>()
                    );
            });

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            //app.UseStaticFiles();
            //app.UseCookiePolicy();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

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
