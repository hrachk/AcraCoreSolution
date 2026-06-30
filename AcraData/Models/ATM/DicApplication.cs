using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.ATM
{
    public partial class DicApplication
    {
        [Key]
        [Column("AppID", TypeName = "int(11)")]
        public int AppId { get; set; }
        [StringLength(255)]
        public string AppName { get; set; }
    }
}
