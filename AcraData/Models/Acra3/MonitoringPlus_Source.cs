using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class MonitoringPlus_Source
    {
        [Key]
        [Column("SourceID", TypeName = "int(11)")]
        public int SourceID { get; set; }       

        [Column("ResultSourceID", TypeName = "int(11)")]
        public int ResultSourceID { get; set; }

        [Column("Status", TypeName = "int(4)")]
        public int Status { get; set; }
    }
}
