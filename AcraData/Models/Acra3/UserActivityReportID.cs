using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class UserActivityReportID
    {
        [Key]
        [Column(TypeName = "int(11)")]
        public int UserActivityId { get; set; }
        [Column("UserActivityReportName")]
        [StringLength(30)]       
        public string UserActivityReportName { get; set; }
        [Column("ReportUrl")]
        [StringLength(250)]       
        public string ReportUrl { get; set; }       
    }
}
