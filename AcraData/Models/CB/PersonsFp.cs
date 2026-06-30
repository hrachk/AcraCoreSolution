using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class PersonsFP
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FamilyName { get; set; }
        public bool IsPe { get; set; }
        public string Ssn { get; set; }
        public bool HasNoSsncertificate { get; set; }
        public int Gender { get; set; }
        public string ResidencyCountry { get; set; }
        public int? EmploymentStatus { get; set; }
        public int? Education { get; set; }
        public int? MartialStatus { get; set; }
        public int? FamilyMembers { get; set; }
        public decimal? IncomesPersonal { get; set; }
        public decimal? IncomesFamily { get; set; }
        public decimal? IncomesAbroad { get; set; }
        public DateTime DateOfBirth { get; set; }

        public Person Person { get; set; }
    }
}
