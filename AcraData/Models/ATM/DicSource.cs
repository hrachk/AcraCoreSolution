using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.ATM
{
    public partial class DicSource
    {
        [Key]
        [Column("SourceID", TypeName = "int(11)")]
        public int SourceId { get; set; }
        [StringLength(255)]
        public string SourceCode { get; set; }
    }
}
