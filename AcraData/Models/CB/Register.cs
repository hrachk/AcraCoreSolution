using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class Register
    {
        public Register()
        {
            A002s = new HashSet<A002>();
        }

        public long Id { get; set; }
        public long A002Id { get; set; }
        public string SocCard { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public DateTime? DateFrom { get; set; } // DateTime?
        public DateTime? DateTo { get; set; }// DateTime?
        public string Department { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Nationality { get; set; }
        public string Sex { get; set; }
        public string FirstNameEng { get; set; }
        public string SecondNameEng { get; set; }
        public string LastNameEng { get; set; }
        public string BirthAddress { get; set; }
        public string BirthCommunity { get; set; }
        public string BirthCountryCode { get; set; }
        public string BirthCoountryName { get; set; }
        public DateTime? BirthDate { get; set; }// DateTime?
        public string BirthRegion { get; set; }
        public string BirthResidence { get; set; }
        public string CitizensCountryCode { get; set; }
        public string CitizensCountryName { get; set; }
        public string PresidentOrder { get; set; }
        public string CertificateNumber { get; set; }
        public short? IsDead { get; set; }
        public DateTime? DeathDate { get; set; }// DateTime?
        public short? SsnIndicator { get; set; }

        public A002 A002 { get; set; }
        public ICollection<A002> A002s { get; set; }
    }
}
