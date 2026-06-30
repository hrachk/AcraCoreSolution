using System;
using System.Collections.Generic;

namespace AcraData.Models.Trigger
{
    public partial class TriggerPerson
    {
        public TriggerPerson()
        {
            TriggerPersonsDetails = new HashSet<TriggerPersonsDetail>();
        }

        public long Id { get; set; }
        public int? Tsid { get; set; }
        public int PersonId { get; set; }
        public int PersonType { get; set; }
        public DateTime SysDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Status { get; set; }


        public TriggerSource TriggerSource { get; set; }
        public ICollection<TriggerPersonsDetail> TriggerPersonsDetails { get; set; }
    }
}
