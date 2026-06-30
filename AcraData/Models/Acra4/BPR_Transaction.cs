namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("BPR_Transaction")]
    public partial class BPR_Transaction
    {
        [Key]
        public long ID { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
        public string Request { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
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
