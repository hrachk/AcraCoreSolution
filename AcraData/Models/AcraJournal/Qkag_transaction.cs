using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.AcraJournal
{
    public partial class Qkag_transaction
    {
        [Key]
        public long ID { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
        public string Request { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
        public string Response { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime? ResponseDateTime { get; set; }
    }
}
