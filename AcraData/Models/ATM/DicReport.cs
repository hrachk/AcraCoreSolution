using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.ATM
{
    public partial class DicReport
    {
        [Key]
        [Column("ReportID", TypeName = "int(11)")]
        public int ReportId { get; set; }
        [StringLength(255)]
        public string ReportName { get; set; }
    }
}
