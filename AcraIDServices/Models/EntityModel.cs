using System;
using System.Collections.Generic;
using System.Text;

namespace AcraIDServices.Models
{
    public class PersonInfo
    {
        public int PersonId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string SocialCard { get; set; }

        public string DocumentNum { get; set; }
   
        public string BirthDate { get; set; }
    }

    public class OrgInfo
    {
        public int OrgId { get; set; }

        public string HVHH { get; set; }
    }
}
