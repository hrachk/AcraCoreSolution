using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.ATM
{
    public partial class ActivitySearchPerson
    {
        [Key]
        [Column(TypeName = "bigint(20)")]
        public long AutoNumber { get; set; }
        [Column("ActivityID", TypeName = "bigint(20)")]
        public long ActivityId { get; set; }
        [Column("PersonID", TypeName = "bigint(20)")]
        public long? PersonId { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("ActivitySearchPersons")]
        public ActivityLog Activity { get; set; }
    }
}
