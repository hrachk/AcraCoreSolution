using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class MonitoringPlusByAcraID
    {
        [Key]
        [Column("AutoID", TypeName = "int(11)")]
        public int AutoID { get; set; }       

        [Column("ACRAID", TypeName = "int(11)")]
        public int ACRAID { get; set; }

        [Column(TypeName = "date")]
        public DateTime? InfoDate { get; set; }

        public string Report { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ModifyDate { get; set; }    
    }
}
