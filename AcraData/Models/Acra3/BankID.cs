using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class BankIDs
    {
        
        public string BankID { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
       
        public string PassportNum { get; set; }
       
        public string SocialCard { get; set; }
       
        public string HasNSocialCard { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }     
    }
}
