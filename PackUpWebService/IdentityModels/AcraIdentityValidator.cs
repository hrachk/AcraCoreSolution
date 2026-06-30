using AcraData.Data;
using AcraData.Models.Acra3;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CheckUpWebService.IdentityModels
{
    public class AcraIdentityValidator
    {
        DbContextOptions<Acra3DbContext> _options;
        public AcraIdentityValidator(DbContextOptions<Acra3DbContext> options)
        {
            _options = options;
        }

        public bool ValidateUser(string userName, string password)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] passwordHash = sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            UserInfo userInfo;
            using (Acra3DbContext ctx = new Acra3DbContext(_options))
            {
                userInfo = Queryable.Where(ctx.UserInfos,u => u.UserLogin == userName).SingleOrDefault();
            }
            if (userInfo == null)
            {
                return false;
            }
            if (userInfo.UserPassword.Equals(string.Join("", sha1.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)).Select(b => b.ToString("x2")).ToArray())))
            {                
                return true;
            }

            return false;
        }
    }
}
