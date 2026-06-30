using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class LoanPawn
    {
        public long Id { get; set; }
        public long LoanId { get; set; }
        public string ExternalId { get; set; }
        public int Subject { get; set; }
        public string CurrencyCode { get; set; }
        public decimal EstimatedValue { get; set; }
        public string Notes { get; set; }
        public long RefId { get; set; }
        public bool? IsDeleted { get; set; }

        public Loan Loan { get; set; }
    }
}
