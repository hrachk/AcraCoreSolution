using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class Loan
    {
        public Loan()
        {
            LoanAffiliates = new HashSet<LoanAffiliate>();
            LoanCoDebtors = new HashSet<LoanCoDebtor>();
            LoanCollaterals = new HashSet<LoanCollateral>();
            LoanDetails = new HashSet<LoanDetail>();
            LoanGuarantors = new HashSet<LoanGuarantor>();
            LoanModificationDates = new HashSet<LoanModificationDate>();
            LoanOwners = new HashSet<LoanOwner>();
            LoanPawns = new HashSet<LoanPawn>();
            OverdueDaysOfLoans = new HashSet<OverdueDaysOfLoan>();
        }

        public long Id { get; set; }
        public long? RefId { get; set; }
        public string CreditCode { get; set; }
        public long DebtorId { get; set; }
        public bool IsPe { get; set; }
        public string ContractNumber { get; set; }
        public string UseField { get; set; }
        public int? UsePurpose { get; set; }
        public string UseCountry { get; set; }
        public string UseRegion { get; set; }
        public string OldCreditCode { get; set; }
        public string InterOrg { get; set; }
        public string InterProgram { get; set; }
        public bool? IsDeleted { get; set; }
        public long? DelRefId { get; set; }
        public string DeleteReason { get; set; }
        public DateTime? ContractDate { get; set; }

        public ICollection<LoanAffiliate> LoanAffiliates { get; set; }
        public ICollection<LoanCoDebtor> LoanCoDebtors { get; set; }
        public ICollection<LoanCollateral> LoanCollaterals { get; set; }
        public ICollection<LoanDetail> LoanDetails { get; set; }
        public ICollection<LoanGuarantor> LoanGuarantors { get; set; }
        public ICollection<LoanModificationDate> LoanModificationDates { get; set; }
        public ICollection<LoanOwner> LoanOwners { get; set; }
        public ICollection<LoanPawn> LoanPawns { get; set; }
        public ICollection<OverdueDaysOfLoan> OverdueDaysOfLoans { get; set; }
    }
}
