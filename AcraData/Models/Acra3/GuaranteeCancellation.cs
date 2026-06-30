using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AcraData.Models.Acra3
{
    public partial class GuaranteeCancellation
    {
        [Key]
        public int GuaranteeCancellationID { get; set; }
        public int GuarantorID { get; set; }
        public int UserID { get; set; }
        public Nullable<System.DateTime> CancellationDate { get; set; }
        public string Note { get; set; }
        public sbyte Status { get; set; }
        public System.DateTime IncomingDate { get; set; }

        //public Guarantor Guarantor { get; set; }
    }
}
