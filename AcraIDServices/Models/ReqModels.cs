using System;
using System.Collections.Generic;
using System.Text;

namespace AcraIDServices.Models
{  
    public class ByName
    {       
        public string type = "name";
        public string name { get; set; }
        public string surname { get; set; }
        public string dob { get; set; }
    }

    public class BySSN
    {          
        public string ssn { get; set; }        
    }

    public class ByDocument
    {        
        public string type = "doc_name";
        public string documentNumber { get; set; }       
    }
}
