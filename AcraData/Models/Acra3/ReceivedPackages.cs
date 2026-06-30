using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class ReceivedPackage
    {
        [Key]
        [Column("RPID", TypeName = "int(11)")]
        public int RPId { get; set; }
        [Column("SessionID", TypeName = "varchar(100)")]
        public string SessionId { get; set; }
        [Column("UserID", TypeName = "int(11)")]
        public int UserId { get; set; }
        [Column("PackageSourceID", TypeName = "int(11)")]
        public int PackageSourceId { get; set; }
        [StringLength(100)]
        public string FileName { get; set; }
        public sbyte FileStatus { get; set; }
        [Column(TypeName = "date")]
        public DateTime? UploadDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }
        [StringLength(255)]
        public string ErrorMessage { get; set; }
    }
}
