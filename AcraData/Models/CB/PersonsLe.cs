using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class PersonsLE
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public string Name { get; set; }
        public string ResidencyCountry { get; set; }
        public string ExecutiveDirectorBankId { get; set; }
        public int? OwnershipTypeId { get; set; }
        public int? LegalTypeId { get; set; }
        public decimal? Incomes { get; set; }

        public Person Person { get; set; }
    }
}
