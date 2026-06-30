using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AcraData.Models.Acra3
{
    public partial class CreditOwner
    {
        [Key]
     //   [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LineID { get; set; }
        public long CreditID { get; set; }
        public string GroupID { get; set; }
        public Nullable<int> PersonID { get; set; }
        public Nullable<int> OrganizationID { get; set; }
        public Nullable<float> Percent { get; set; }
        public Nullable<int> Status { get; set; }
        public Nullable<int> SourceID { get; set; }
        public Nullable<System.DateTime> IncomingDate { get; set; }
        public Nullable<System.DateTime> StatusModifyDate { get; set; }

      //  public Credit Credit { get; set; }

    }
}
