using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class UserInfo
    {
        [Key]
        [Column("UserID", TypeName = "int(11)")]
        public int UserId { get; set; }
        [StringLength(50)]
        public string UserLogin { get; set; }
        [StringLength(50)]
        public string UserPassword { get; set; }
        [Column(TypeName = "date")]
        public DateTime? UserPassCreationDate { get; set; }
        [Column(TypeName = "int(11)")]
        public int? Position { get; set; }
        [Column("eMail")]
        [StringLength(200)]
        public string EMail { get; set; }
        [StringLength(200)]
        public string Initials { get; set; }
        [Column(TypeName = "int(11)")]
        public int? Type { get; set; }
        [Column(TypeName = "int(11)")]
        public int? ClientId { get; set; }
        [Column(TypeName = "date")]
        public DateTime? CreationTime { get; set; }
        [Column("IPAddress")]
        [StringLength(160)]
        public string Ipaddress { get; set; }
        [Column(TypeName = "int(11)")]
        public int? Status { get; set; }
        [Column(TypeName = "date")]
        public DateTime? PassiveToDate { get; set; }
    }
}
