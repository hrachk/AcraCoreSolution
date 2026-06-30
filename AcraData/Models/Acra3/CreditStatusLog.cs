using System;
using System.Collections.Generic;
using System.Text;

namespace AcraData.Models.Acra3
{
    public partial class CreditStatusLog
    {
        public long Id { get; set; }
        public long CreditId { get; set; }
        public int FirstState_InternalID { get; set; }
        public int OldValue { get; set; }
        public int NewValue { get; set; }
        public DateTime StatusModifyDate { get; set; }
    }
}
