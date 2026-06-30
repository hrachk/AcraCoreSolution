using System.Collections.Generic;
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
using AutoMapper.Data;
using Microsoft.Extensions.Options;
using idunno.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CheckUpBE
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
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

            services.AddDbContext<Acra3DbContext>(options =>
                     options.UseMySql(Configuration.GetConnectionString("Acra3Connection")));
            services.AddOptions();

            services.AddSingleton(serviceProvider =>
            {
                serviceProvider = services.BuildServiceProvider();
                return new CheckUpService.CheckUpBackService(
                   serviceProvider.GetRequiredService<DbContextOptions<Acra3DbContext>>(),
                   serviceProvider.GetRequiredService<Logger>()
                    );
            });


        
            services.AddAutoMapper(cfg => cfg.AddDataReaderMapping());
            services.AddMvc();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
