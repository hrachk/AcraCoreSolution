using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcraIDServices.Models
{
    public class PersonWithoutResidency
    {
        public string passport { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string birthDate { get; set; }
        public string ssn { get; set; }
        public string idCard { get; set; }
    }
}
