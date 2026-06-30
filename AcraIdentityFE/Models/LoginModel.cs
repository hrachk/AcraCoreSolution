using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcraIdentityFE.Models
{
    public class LoginModel
    {        
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string Scope { get; set; }
    }
}
