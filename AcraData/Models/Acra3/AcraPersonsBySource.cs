using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class AcraPersonsBySource
    {
        [Key]
        [Column("AutoID", TypeName = "int(11)")]
        public int AutoID { get; set; }
        [Column("ACRAID", TypeName = "int(11)")]        
        public int ACRAID { get; set; }
        [Column("SourceID", TypeName = "int(11)")]
        public int SourceID { get; set; }
        [Column("Status", TypeName = "int(11)")]
        public int Status { get; set; }
        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }       
        [Column(TypeName = "date")]
        public DateTime? ModifyDate { get; set; }        
    }
}
