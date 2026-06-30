using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckUpService.Models
{
    public class PackUp : IPackUp
    {        
        public string SourceName => throw new NotImplementedException();
        public IFormFile ExcelFile => throw new NotImplementedException();
    }
}
