using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class Passport
    {
        [Key]
        [Column("PassportID", TypeName = "int(11)")]
        public int PassportId { get; set; }
        // [Column("PersonID", TypeName = "int(11) unsigned")]
        [Column("PersonID", TypeName = "int(11)")]
        public int? PersonId { get; set; }
        [StringLength(30)]
        public string PassportNum { get; set; }
        [Column(TypeName = "int(11)")]
        public int? AuthorityOrg { get; set; }
        [Column(TypeName = "date")]
        public DateTime? IssueDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ExpireDate { get; set; }
        [Column(TypeName = "int(11)")]
        public int? Country { get; set; }
        [Column(TypeName = "int(11)")]
        public int? PassportType { get; set; }
        [Column("SourceID", TypeName = "int(11)")]
        public int? SourceId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime IncomingDate { get; set; }

        //[ForeignKey("PersonId")]
        //[InverseProperty("Passports")]
        //public Person Person { get; set; }
    }
}
