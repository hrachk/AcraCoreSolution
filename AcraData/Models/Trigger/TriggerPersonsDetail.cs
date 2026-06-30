using System;
using System.Collections.Generic;

namespace AcraData.Models.Trigger
{
    public partial class TriggerPersonsDetail
    {
        public long Id { get; set; }
        public long? Tpid { get; set; }
        public int PersonId { get; set; }
        public DateTime? SysDate { get; set; }

        public TriggerPerson TriggerPerson { get; set; }
    }
}
