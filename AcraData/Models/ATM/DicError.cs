using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.ATM
{
    public partial class DicError
    {
        [Key]
        [Column(TypeName = "int(11)")]
        public int ErrorCode { get; set; }
        [StringLength(512)]
        public string ErrorText { get; set; }
    }
}
