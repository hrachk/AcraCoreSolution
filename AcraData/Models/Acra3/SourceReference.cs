using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class SourceReference
    {
        [Column("RecordID", TypeName = "int(11)")]
        public int RecordId { get; set; }
        [Column(TypeName = "int(11)")]
        public int ReferenceTable { get; set; }
        [Column(TypeName = "int(11)")]
        public int? Status { get; set; }
        [Column("SourceID", TypeName = "int(11)")]
        public int SourceId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? IncomingDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StatusModifyDate { get; set; }
        [Column("ReceivedPackageID", TypeName = "int(11) unsigned")]
        public uint? ReceivedPackageId { get; set; }
    }
}
