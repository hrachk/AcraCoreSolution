using System;
using System.Collections.Generic;
using System.Text;

namespace AcraIDServices.Models
{
    public class data
    {
        public string PNum { get; set; }
        public string full_name { get; set; }
        public string AVVRegistrationAddress { get; set; }
    }
    public class PNumModel
    {        
        public string opaque { get; set; }

        public data data { get; set; }
    }
}
                               