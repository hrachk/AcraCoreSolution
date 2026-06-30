using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class LoginLog
    {
        [Key]
        [Column("LoginLogID", TypeName = "int(11)")]
        public int LoginLogId { get; set; }
        [StringLength(250)]
        public string UserLogin { get; set; }
        [StringLength(250)]
        public string UserPassword { get; set; }
        [Column("UserID", TypeName = "int(11)")]
        public int? UserId { get; set; }
        [Column("SourceID", TypeName = "int(11)")]
        public int? SourceId { get; set; }
        [Column(TypeName = "int(11)")]
        public int? UserType { get; set; }
        [Column(TypeName = "int(11)")]
        public int? SourceType { get; set; }
        [Column("IPAddress")]
        [StringLength(15)]
        public string Ipaddress { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LoginDateTime { get; set; }
        [Column("SessionID")]
        [StringLength(100)]
        public string SessionId { get; set; }

        /// <summary>
        /// 1 => Success | 2 => Blocked IP | 3 => Password expired 
        /// | 4 => Wrong login/pass | 5 => Wrong captcha code | 6 => Disabled by admin
        /// </summary>
        [Column(TypeName = "tinyint(4)")]
        public sbyte? Type { get; set; }
    }
}
