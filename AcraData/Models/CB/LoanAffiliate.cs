using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class LoanAffiliate
    {
        public long Id { get; set; }
        public long LoanId { get; set; }
        public string AffiliateId { get; set; }
        public string AffiliateNotes { get; set; }
        public bool? IsDeleted { get; set; }
        public long RefId { get; set; }

        public Loan Loan { get; set; }
    }
}
