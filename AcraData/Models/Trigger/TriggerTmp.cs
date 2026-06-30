using System;
using System.Collections.Generic;
using System.Text;

namespace AcraData.Models.Trigger
{
    public partial class TriggerTmp
    {
        public long Id { get; set; }
        public int ActivityId { get; set; }
        public int? ActivityType { get; set; }
        public int Status { get; set; }
    }
}
