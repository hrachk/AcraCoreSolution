using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class Identity
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public int TypeId { get; set; }
        public string Number { get; set; }
        public string IssuingAuthority { get; set; }
        public DateTime? DateOfIssue { get; set; }
        public DateTime? DateOfExpiry { get; set; }

        public Person Person { get; set; }
    }
}
