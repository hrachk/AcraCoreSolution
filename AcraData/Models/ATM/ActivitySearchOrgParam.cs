using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.ATM
{
    public partial class ActivitySearchOrgParam
    {
        [Key]
        [Column("ActivityID", TypeName = "bigint(20)")]
        public long ActivityId { get; set; }
        [Column("OrgID", TypeName = "int(11)")]
        public int? OrgId { get; set; }
        [Column("BankID", TypeName = "bigint(20)")]
        public long? BankId { get; set; }
        [StringLength(255)]
        public string OrgName { get; set; }
        [Column("OrgANTP")]
        [StringLength(255)]
        public string OrgAntp { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("ActivitySearchOrgParams")]
        public ActivityLog Activity { get; set; }
    }
}
