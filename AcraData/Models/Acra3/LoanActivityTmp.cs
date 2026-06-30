using System;
using System.Collections.Generic;
using System.Text;

namespace AcraData.Models.Acra3
{
    public partial class LoanActivityTmp  
    {
        public long Id { get; set; }
        public long CreditID { get; set; }
        public int? ActivityType { get; set; }        
        public int Status { get; set; }
    }
}
