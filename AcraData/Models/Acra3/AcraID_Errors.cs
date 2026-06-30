using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class AcraID_Errors
    {
        [Key]
        [Column("Id", TypeName = "int(11) unsigned")]
        public uint Id { get; set; }
        [Column(TypeName = "int(11)")]
        public uint? PersonId { get; set; }
        [StringLength(50)]
        public string Field { get; set; }
        [StringLength(50)]
        public string Value1 { get; set; }
        [StringLength(50)]
        public string Value2 { get; set; }
        [Column(TypeName = "int(1)")]
        public uint? Isavv { get; set; }
        [Column(TypeName = "int(3)")]
        public uint? Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Date { get; set; }
       
    }
}
