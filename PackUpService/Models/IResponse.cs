using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CheckUpService.Models
{
    public interface IResponse
    {                              
        string ResponseID { get; set; }
        long ResponseTime { get; set; }
        int ErrorCode { get; set; }    
        string ErrorDesc { get; set; }
    }    
}
