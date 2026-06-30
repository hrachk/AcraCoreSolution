using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.ATM
{
    public partial class ActivityLogDetail
    {
        [Key]
        [Column("ActivityID", TypeName = "bigint(20)")]
        public long ActivityId { get; set; }
        [Column(TypeName = "int(11)")]
        public int? PersonType { get; set; }
        [Column("ReportID", TypeName = "int(11)")]
        public int? ReportId { get; set; }
        [Column("ReportReasonID", TypeName = "int(255)")]
        public int? ReportReasonId { get; set; }
        [Column("ReportSubReasonID", TypeName = "int(11)")]
        public int? ReportSubReasonId { get; set; }
        [Column(TypeName = "int(255)")]
        public int? Status { get; set; }
        [Column(TypeName = "bit(1)")]
        public bool? IsMonitoring { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("ActivityLogDetails")]
        public ActivityLog Activity { get; set; }
        [ForeignKey("PersonType")]
        [InverseProperty("ActivityLogDetails")]
        public DicPersonType PersonTypeNavigation { get; set; }
    }
}
