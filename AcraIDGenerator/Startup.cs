
using AcraData.Data;
using AcraUtils;
using Easy.Logger;
using Easy.Logger.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

namespace AcraIDGenerator
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

            services.AddRazorPages();

            services.Configure<AcraUtils.Configuration.AcraIDGeneratorConfig>(Configuration.GetSection("AcraIDGeneratorConfig"));

            AcraUtils.Cryptor cryptor = new AcraUtils.Cryptor();
            services.AddDbContext<AcraData.Data.Acra4DbContext>(options =>
            options.UseMySql(cryptor.DecryptDES(Configuration.GetConnectionString("Acra4Connection")), ServerVersion.AutoDetect(cryptor.DecryptDES(Configuration.GetConnectionString("Acra4Connection")))));

            services.AddDbContext<AcraData.Data.Acra3DbContext>(options =>
            options.UseMySql(cryptor.DecryptDES(Configuration.GetConnectionString("Acra3Connection")), ServerVersion.AutoDetect(cryptor.DecryptDES(Configuration.GetConnectionString("Acra3Connection")))));

            services.AddDbContext<AcraData.Data.AcraJournalDbContext>(options =>
            options.UseMySql(cryptor.DecryptDES(Configuration.GetConnectionString("AcraJournal")), ServerVersion.AutoDetect(cryptor.DecryptDES(Configuration.GetConnectionString("AcraJournal")))));

            services.AddSingleton(serviceProvider =>
            {
                serviceProvider = services.BuildServiceProvider();
                return new AcraIDGenerator.AcraIDGeneratorService(
                    serviceProvider.GetRequiredService<DbContextOptions<Acra3DbContext>>(),
                    serviceProvider.GetRequiredService<DbContextOptions<Acra4DbContext>>(),
                    serviceProvider.GetRequiredService<DbContextOptions<AcraJournalDbContext>>(),
                    serviceProvider.GetRequiredService<IOptions<AcraUtils.Configuration.AcraIDGeneratorConfig>>(),
                    serviceProvider.GetRequiredService<Logger>()
                    );
            });

            services.AddSingleton(Configuration.GetSection("HostOptions").Get<AcraUtils.Configuration.AcraIDConfig>());

            

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
            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("Back", "Back", "back/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            //app.Run(context => context.Response.WriteAsync("<div>AcraID is Running, please Minimize this window</div>"));
        }
    }
}
