using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckUpService.Models
{
    interface IPackUp
    {     
        string SourceName { get; }
        IFormFile ExcelFile { get; }
    }
}
