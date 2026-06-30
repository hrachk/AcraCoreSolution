using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class Pek_ActivityLog
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int id { get; set; }
        [Column("userActivityId", TypeName = "bigint(20)")]
        public long userActivityId { get; set; }
        [StringLength(50)]
        public string message { get; set; }
        [Column(TypeName = "date")]
        public DateTime? date { get; set; }
    }
}
