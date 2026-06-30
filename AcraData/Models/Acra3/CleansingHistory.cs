using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class CleansingHistory
    {
        [Column("CleansingHistoryID", TypeName = "int(11)")]
        public int CleansingHistoryId { get; set; }
        [Column(TypeName = "int(11)")]
        public int? RecipientType { get; set; }
        [Column("RemovedID", TypeName = "int(11)")]
        public int? RemovedId { get; set; }
        [Column("NewID", TypeName = "int(11)")]
        public int? NewId { get; set; }
        [Column("SourceID", TypeName = "int(11)")]
        public int? SourceId { get; set; }
        [Column("ClerckID", TypeName = "int(11)")]
        public int? ClerckId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ModificationDate { get; set; }
    }
}
