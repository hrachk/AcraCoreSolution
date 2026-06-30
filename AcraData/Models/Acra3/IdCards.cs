using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class IdCard
    {
        [Key]
        [Column("IdCardID", TypeName = "int(11)")]
        public int IdCardId { get; set; }
        //[Column("PersonID", TypeName = "int(11) unsigned")]
        [Column("PersonID", TypeName = "int(11)")]
        public int? PersonId { get; set; }
        [StringLength(30)]
        public string IdCardNum { get; set; }
        [Column(TypeName = "int(11)")]
        public int? AuthorityOrg { get; set; }
        [Column(TypeName = "date")]
        public DateTime? IssueDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ExpireDate { get; set; }
        [Column("SourceID", TypeName = "int(11)")]
        public int? SourceId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime IncomingDate { get; set; }

        //[ForeignKey("PersonId")]
        //public Person Person { get; set; }
    }
}
