using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class UserActivityLog
    {
        //public UserActivityLog()
        //{
        //    UserActivityParams = new HashSet<UserActivityParam>();
        //}

        [Key]
        [Column(TypeName = "int(11)")]
        public int UserActivityId { get; set; }
        [Column("UserID", TypeName = "int(11)")]
        public int UserId { get; set; }
        [Column(TypeName = "int(11)")]
        public int ActivityType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ActivityTime { get; set; }
        [Column(TypeName = "tinyint(1)")]
        public bool? CleansingStatus { get; set; }

        //[InverseProperty("UserActivityLog")]
      //  public ICollection<UserActivityParam> UserActivityParams { get; set; }
    }
}
