using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class Pek_Definition
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int id { get; set; }
        [StringLength(50)]
        public string parameter { get; set; }
        [StringLength(50)]
        public string acceptablevalue { get; set; }
    }
}
