using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.ATM
{
    public partial class DicPersonType
    {
        public DicPersonType()
        {
            ActivityLogDetails = new HashSet<ActivityLogDetail>();
        }

        [Key]
        [Column("PersonTypeID", TypeName = "int(11)")]
        public int PersonTypeId { get; set; }
        [StringLength(255)]
        public string PersonType { get; set; }

        [InverseProperty("PersonTypeNavigation")]
        public ICollection<ActivityLogDetail> ActivityLogDetails { get; set; }
    }
}
