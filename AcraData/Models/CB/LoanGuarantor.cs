using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class LoanGuarantor
    {
        public long Id { get; set; }
        public long LoanId { get; set; }
        public string GuarantorId { get; set; }
        public string GuarantyCurrency { get; set; }
        public decimal? GuarantyAmount { get; set; }
        public string GuarantorNotes { get; set; }
        public bool? IsDeleted { get; set; }
        public long RefId { get; set; }

        public Loan Loan { get; set; }
    }
}
