using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class LoanOwner
    {
        public long Id { get; set; }
        public long LoanId { get; set; }
        public string OwnerId { get; set; }
        public decimal? Proportion { get; set; }
        public string OwnerNotes { get; set; }
        public bool? IsDeleted { get; set; }
        public long RefId { get; set; }

        public Loan Loan { get; set; }
    }
}
