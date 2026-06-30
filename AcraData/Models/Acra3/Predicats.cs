using System;
using System.Collections.Generic;

namespace AcraData.Models.Acra3
{
    public partial class Predicat
    {
        public int ID { get; set; }
        public int PersonId { get; set; }
        public int FirstNameID { get; set; }
        public int LastNameID { get; set; }      
        public DateTime? BirthDate { get; set; }
        //   [Column(TypeName = "varchar")]
        public string PassportNum { get; set; }
       // [Column(TypeName = "varchar")]
        public string IdCardNum { get; set; }
       // [Column(TypeName = "varchar")]
        public string SocialCard { get; set; }
        public long? Criteria1 { get; set; }
        public string SC1 { get; set; }
        public long? Criteria2 { get; set; }
        public string SC2 { get; set; }
        public long? Criteria3 { get; set; }
        public string SC3 { get; set; }
        public long? Criteria4 { get; set; }
        public string SC4 { get; set; }
        public long? Criteria5 { get; set; }
        public string SC5 { get; set; }
        public long? Criteria6 { get; set; }
        public string SC6 { get; set; }
        public long? Criteria7 { get; set; }
        public string SC7 { get; set; }
        public long? Criteria8 { get; set; }
        public string SC8 { get; set; }
        public long? Criteria9 { get; set; }
        public string SC9 { get; set; }
        public long? Criteria10 { get; set; }
        public string SC10 { get; set; }
        public long? Criteria11 { get; set; }
        public string SC11 { get; set; }
        public string AcraGroup { get; set; }
        public bool? IsDeleted { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
