using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class OverdueDaysOfLoan
    {
        public long Id { get; set; }
        public string CreditCode { get; set; }
        public int? OverdueDaysOfMonth { get; set; }
        public long? LoanId { get; set; }
        public long? RefId { get; set; }

        public Loan Loan { get; set; }
    }
}
