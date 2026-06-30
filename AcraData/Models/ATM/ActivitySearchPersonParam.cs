using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.ATM
{
    public partial class ActivitySearchPersonParam
    {
        [Key]
        [Column("ActivityID", TypeName = "bigint(20)")]
        public long ActivityId { get; set; }
        [Column("PersonID", TypeName = "int(11)")]
        public int? PersonId { get; set; }
        [Column("BankID", TypeName = "bigint(20)")]
        public long? BankId { get; set; }
        [StringLength(255)]
        public string FirstName { get; set; }
        [StringLength(255)]
        public string LastName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateOfBirth { get; set; }
        [StringLength(255)]
        public string PassportNumber { get; set; }
        [Column("IDCardNumber")]
        [StringLength(255)]
        public string IdCardNumber { get; set; }
        [Column("SSNumber")]
        [StringLength(255)]
        public string SSNumber { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("ActivitySearchPersonParams")]
        public ActivityLog Activity { get; set; }
    }
}
