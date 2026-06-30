using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class LoanCollateral
    {
        public long Id { get; set; }
        public long LoanId { get; set; }
        public int ExternalId { get; set; }
        public long RefId { get; set; }
        public bool? IsDeleted { get; set; }

        public Loan Loan { get; set; }
    }
}
