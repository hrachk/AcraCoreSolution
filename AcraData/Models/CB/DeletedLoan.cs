using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class DeletedLoan
    {
        public long Id { get; set; }
        public string CreditCode { get; set; }
        public string DeleteReason { get; set; }
        public long? LoanId { get; set; }

        public L003 L003 { get; set; }
    }
}
