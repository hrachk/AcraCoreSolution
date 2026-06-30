using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AcraData.Models.Acra3
{ 
    public partial class Credit :ICloneable
    {
        [Key]
        public int InternalID { get; set; }
        public long CreditID { get; set; }
        public string ExternalID { get; set; }
        public Nullable<int> RecipientType { get; set; }
        public Nullable<int> CreditStatus { get; set; }
        public Nullable<int> CreditClassification { get; set; }
        public Nullable<int> CreditType { get; set; }
        public Nullable<System.DateTime> CreditStart { get; set; }
        public Nullable<System.DateTime> ActualCreditStart { get; set; }
        public Nullable<System.DateTime> FirstInstallment { get; set; }
        public Nullable<System.DateTime> LastInstallment { get; set; }
        public Nullable<System.DateTime> LastPaymentDate { get; set; }
        public Nullable<int> InstallmentsNumber { get; set; }
        public Nullable<long> OutstandingPercent { get; set; }
        public Nullable<System.DateTime> OutstandingDate { get; set; }
        public Nullable<long> ContractAmount { get; set; }
        public Nullable<long> CreditAmount { get; set; }
        public Nullable<long> PaymentAmount { get; set; }
        public Nullable<long> AmountDue { get; set; }
        public Nullable<long> AmountOverdue { get; set; }
        public Nullable<long> CollateralPrice { get; set; }
        public Nullable<int> ProlongationsNum { get; set; }
        public Nullable<int> Currency { get; set; }
        public Nullable<double> AnnualRate { get; set; }
        public Nullable<int> CreditScopeID { get; set; }
        public Nullable<int> CreditUsePlace { get; set; }
        public string CreditRegistryCode { get; set; }
        public Nullable<int> OverdueDays { get; set; }
        public Nullable<System.DateTime> ClassificationLastDate { get; set; }
        public string CreditNotes { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> SourceID { get; set; }
        public Nullable<System.DateTime> IncomingDate { get; set; }
        public Nullable<System.DateTime> StatusModifyDate { get; set; }
        public Nullable<long> ReceivedPackageID { get; set; }

        public ICollection<CreditOwner> CreditOwners { get; set; }
        public ICollection<Guarantor> Guarantors { get; set; }

        public virtual object Clone() { return this.MemberwiseClone(); }

    }
}
