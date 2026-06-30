using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class LoanModificationDate
    {
        public long Id { get; set; }
        public long? LoanId { get; set; }
        public long RefId { get; set; }
        public DateTime? ModificationDateTime { get; set; }

        public Loan Loan { get; set; }
    }
}
