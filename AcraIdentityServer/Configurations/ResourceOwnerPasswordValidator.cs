using AcraData.Data;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace AcraIdentityServer.Configurations
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        DbContextOptions<Acra3DbContext> _options;
        private IEventService _events;
        public ResourceOwnerPasswordValidator(DbContextOptions<Acra3DbContext> options, IEventService events) : base()
        {
            _options = options;
            _events = events;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider();

            using (Acra3DbContext ctx = new Acra3DbContext(_options))
            {
                var userInfo = Queryable.Where(ctx.UserInfos, u => u.UserLogin == context.UserName).SingleOrDefault();

                string pass = string.Join("", sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(context.Password)).Select(b => b.ToString("x2")).ToArray());
                string pass2 = string.Join("", sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(context.Password)).Select(b => b.ToString("x2")).ToArray());
                if ((userInfo == null) || !(userInfo.UserPassword.Equals(string.Join("", sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(context.Password)).Select(b => b.ToString("x2")).ToArray()))))
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidClient, $"User name or password is incorrect. {ctx.Database.GetDbConnection().ConnectionString}");
                    return Task.FromResult(0);
                }
                context.Result = new GrantValidationResult(userInfo.ClientId.ToString(), "password");
                return Task.FromResult(0);

            }
        }
    }
}
