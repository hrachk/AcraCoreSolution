using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.ATM
{
    public partial class ActivityOrg
    {
        [Key]
        [Column(TypeName = "bigint(20)")]
        public long AutoNumber { get; set; }
        [Column("ActivityID", TypeName = "bigint(20)")]
        public long ActivityId { get; set; }
        [Column("OrgID", TypeName = "bigint(20)")]
        public long? OrgId { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("ActivityOrgs")]
        public ActivityLog Activity { get; set; }
    }
}
