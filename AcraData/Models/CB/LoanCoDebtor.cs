using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class LoanCoDebtor
    {
        public long Id { get; set; }
        public long LoanId { get; set; }
        public string CoDebtorId { get; set; }
        public decimal? Proportion { get; set; }
        public string CoDebtorNotes { get; set; }
        public bool? IsDeleted { get; set; }
        public long RefId { get; set; }

        public Loan Loan { get; set; }
    }
}
