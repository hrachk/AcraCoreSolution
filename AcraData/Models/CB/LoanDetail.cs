using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class LoanDetail
    {
        public long Id { get; set; }
        public long? LoanId { get; set; }
        public long RefId { get; set; }
        public int? LoanTypeId { get; set; }
        public int? LoanStatus { get; set; }
        public int? ContractTypeId { get; set; }
        public int? InterestRateTypeId { get; set; }
        public int? RevisionReasonId { get; set; }
        public int? RiskId { get; set; }
        public int? ConditionsChangeCount { get; set; }
        public int? RepaymentSourceId { get; set; }
        public int? OverdueDays { get; set; }
        public int? RevisedDays { get; set; }
        public bool AffectionWithCreditor { get; set; }
        public bool IsInterestSubsidy { get; set; }
        public decimal? ContractAmount { get; set; }
        public decimal? ContractModifiedAmount { get; set; }
        public decimal? SubsidyAmount { get; set; }
        public decimal? AnnualInterestRate { get; set; }
        public decimal? ActualInterestRate { get; set; }
        public decimal? AmountOff { get; set; }
        public decimal? AmountsPaid { get; set; }
        public decimal? CalculatedOtherObligations { get; set; }
        public decimal? CalculatedPenalties { get; set; }
        public decimal? OverduePercent { get; set; }
        public decimal? OverduePrincipalAmount { get; set; }
        public decimal? PrincipalAmount { get; set; }
        public decimal? PercentsPaid { get; set; }
        public string Currency { get; set; }
        public string Notes { get; set; }
        public string DebtorNotes { get; set; }
        public DateTime? GrantingDate { get; set; }
        public DateTime? LastClassificationDate { get; set; }
        public DateTime? LastExpirationDate { get; set; }
        public DateTime? RepaymentDate { get; set; }
        public DateTime? RepaymentActualDate { get; set; }
        public DateTime? RevisionDate { get; set; }

        public Loan Loan { get; set; }
    }
}
