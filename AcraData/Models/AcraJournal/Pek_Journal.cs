namespace AcraData.Models.AcraJournal
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Pek_Journal")]
    public partial class Pek_Journal
    {
        [Key]
        public long ID { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
        public string Request { get; set; }

        [Column(TypeName = "mediumtext")]
        [StringLength(16777215)]
        public string Response { get; set; }

        [Column(TypeName = "bit")]
        public bool IsDeserialized { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
        public string ErrorText { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? ResponseDateTime { get; set; }
        [Column(TypeName = "bigint")]
        public long UserActivityId { get; set; }
        [Column(TypeName = "int(3)")]
        public int SourceID { get; set; }
        [Column(TypeName = "int(3)")]
        public int Status { get; set; }
    }
}
