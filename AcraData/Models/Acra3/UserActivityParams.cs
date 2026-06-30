using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class UserActivityParam
    {
        [Column(TypeName = "int(11)")]
        public int UserActivityId { get; set; }
        [Column("UserActivityParamID", TypeName = "smallint(4)")]        
        public short UserActivityParamId { get; set; }
        [Column(TypeName = "int(11)")]
        public int UserActivityParamValue { get; set; }
      //  [Column(TypeName = "tinyint(4) unsigned")]
        public sbyte? Status { get; set; }
        //[Column("ID", TypeName = "int(11)")]
        //public int Id { get; set; }

        //[ForeignKey("UserActivityId")]
       // public UserActivityLog UserActivityLog { get; set; }
    }
}
