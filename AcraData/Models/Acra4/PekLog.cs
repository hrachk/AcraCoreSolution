namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PekJournal")]
    public partial class PekJournal
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
    }
}
