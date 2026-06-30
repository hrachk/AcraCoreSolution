using System;
using System.Collections.Generic;
using System.Text;

namespace AcraIDServices.Models.AVV
{  
    public class ByName
    {               
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string middle_name { get; set; }
        public string birth_date { get; set; } // yyyy-MM-dd
        public Addresses? Addresses { get; set; } //by Default CURRENT
    }

    public class BySSN
    {          
        public string psn { get; set; }
        public Addresses? Addresses { get; set; } //by Default CURRENT
    }

    public class ByDocument
    {                
        public string docnum { get; set; }
        public Addresses? Addresses { get; set; } //by Default CURRENT
    }

    public enum Addresses
    {
        CURRENT, 
        ALL
    } 
}
