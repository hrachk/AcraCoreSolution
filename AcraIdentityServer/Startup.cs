using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using AcraData.Data;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.Services;
using AcraUtils;
using MySql.Data.MySqlClient;
using System;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace AcraIdentityServer
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {


            //var credential = new SigningCredentials(new X509SecurityKey(GetServerCertificate(Configuration.GetSection("ServerCertificateSKI").Value)), "RS256");


            //File.AppendAllText("C:/Logs/IdentityServer.txt", $"{DateTime.Now} Configure Service Started" + Environment.NewLine);
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                //.AddSigningCredential(credential)
                .AddInMemoryApiResources(Configurations.ApiResources.GetApiResources())
                .AddInMemoryClients(Configurations.Clients.GetClients())
                .AddInMemoryApiScopes(Configurations.ApiScopes.GetApiScopes())
                .AddSecretParser<X509CertificateSecretParser>()
                .AddSecretValidator<X509CertificateSecretValidator>();


            //AcraUtils.Cryptor cryptor = new AcraUtils.Cryptor();
            AcraUtils.Cryptor cryptor = new Cryptor();
            string converted = Configuration.GetConnectionString("Acra3Connection").ToString().Replace('-', '+');
            converted = converted.Replace('_', '/');
            string conn_string = ConnectionBuilder(cryptor.DecryptDES(converted));
            //File.AppendAllText("C:/Logs/IdentityServer.txt", $"{DateTime.Now} Connection Build Done" + Environment.NewLine);
            //MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
            //conn_string.Server = "10.10.2.220";
            //conn_string.UserID = "Acwb3usrA";
            //conn_string.Password = "AcwWwrA3";
            //conn_string.Database = "ACRA3";
            //conn_string.PersistSecurityInfo = true;
            //conn_string.ConnectionTimeout = 180;
            //conn_string.ConvertZeroDateTime = true;
            //conn_string.TreatTinyAsBoolean = false;

            /*conn_string.Server = "localhost";
            conn_string.UserID = "root";
            conn_string.Password = "1234";
            conn_string.Database = "ACRA3";
            conn_string.Port = 3306;
            conn_string.PersistSecurityInfo = true;
            conn_string.ConnectionTimeout = 180;
            conn_string.ConvertZeroDateTime = true;
            conn_string.TreatTinyAsBoolean = false;*/

            services.AddDbContext<Acra3DbContext>(options =>
            options.UseMySql(conn_string.ToString(), ServerVersion.AutoDetect(conn_string.ToString())));

            //services.AddDbContext<Acra3DbContext>(options =>
            //options.UseMySql(cryptor.DecryptDES(Configuration.GetConnectionString("Acra3Connection"))));

            services.AddTransient<IResourceOwnerPasswordValidator, AcraIdentityServer.Configurations.ResourceOwnerPasswordValidator>();
            services.AddTransient<IProfileService, AcraIdentityServer.Configurations.ProfileService>();



            services.AddRazorPages();
            //File.AppendAllText("C:/Logs/IdentityServer.txt", $"{DateTime.Now} Connection to ACRA3 Done" + Environment.NewLine);

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIdentityServer();
            //app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute("Back", "Back", "back/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

        //public static X509Certificate2 GetServerCertificate(string serverCertificateSKI)
        //{
        //    X509Store store = new X509Store(StoreName.TrustedPeople, StoreLocation.CurrentUser);
        //    store.Open(OpenFlags.ReadOnly);
        //    //var collection = store.Certificates.Find(X509FindType.FindBySubjectKeyIdentifier, serverCertificateSKI, true);
        //    //var collection = store.Certificates.Find(X509FindType.FindBySubjectKeyIdentifier, "66 6a 47 b2 e3 a8 25 39 95 4e 7c 2f 2e 2f d4 5e 22 1a 04 87", true);
        //    var collection = store.Certificates;
        //    store.Close();
        //    if (collection.Count > 0)
        //    {
        //        return collection[0];
        //    }

        //    return null;
        //}
        public string ConnectionBuilder(string connString)
        {
            string server = connString.Substring(connString.IndexOf("Server") + 7, connString.IndexOf(";", connString.IndexOf("Server") + 7) - (connString.IndexOf("Server") + 7));
            string username = connString.Substring(connString.IndexOf("User Id") + 8, connString.IndexOf(";", connString.IndexOf("User Id") + 8) - (connString.IndexOf("User Id") + 8));
            string password = connString.Substring(connString.IndexOf("Password") + 9, connString.IndexOf(";", connString.IndexOf("Password") + 9) - (connString.IndexOf("Password") + 9));
            string dataBase = connString.Substring(connString.IndexOf("Database") + 9, connString.IndexOf(";", connString.IndexOf("Database") + 9) - (connString.IndexOf("Database") + 9));
            string presistSecurityInfo = connString.Substring(connString.IndexOf("Persist Security Info") + 22, connString.IndexOf(";", connString.IndexOf("Persist Security Info") + 22)
                - (connString.IndexOf("Persist Security Info") + 22));
            string connectionTimeout = connString.Substring(connString.IndexOf("Connection Timeout") + 19, connString.IndexOf(";", connString.IndexOf("Connection Timeout") + 19)
                - (connString.IndexOf("Connection Timeout") + 19));
            string convertZeroDatetime = connString.Substring(connString.IndexOf("Convert Zero Datetime") + 22, connString.IndexOf(";", connString.IndexOf("Convert Zero Datetime") + 22)
                - (connString.IndexOf("Convert Zero Datetime") + 22));
            string treatTinyAsBoolean = connString.Substring(connString.IndexOf("Treat Tiny As Boolean") + 22, connString.Length
                - (connString.IndexOf("Treat Tiny As Boolean") + 22));

            MySqlConnectionStringBuilder conn_string = new MySqlConnectionStringBuilder();
            conn_string.Server = server;
            conn_string.UserID = username;
            conn_string.Password = password;
            conn_string.Database = dataBase;
            conn_string.PersistSecurityInfo = presistSecurityInfo == "True" ? true : false;
            int.TryParse(connectionTimeout, out int num);
            conn_string.ConnectionTimeout = (uint)num;
            conn_string.ConvertZeroDateTime = convertZeroDatetime == "True" ? true : false;
            conn_string.TreatTinyAsBoolean = treatTinyAsBoolean == "True" ? true : false;

            return conn_string.ToString();
        }
    }
}
