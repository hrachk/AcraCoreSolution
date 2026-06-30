using System;
using System.Collections.Generic;

namespace AcraData.Models.Trigger
{
    public partial class TriggerSource
    {
        public TriggerSource()
        {
            TriggerPersons = new HashSet<TriggerPerson>();
            TriggerReports = new HashSet<TriggerReport>();
            TriggerVolumes = new HashSet<TriggerVolume>();
        }

        public int Id { get; set; }
        public int SourceId { get; set; }
        public int Status { get; set; }
        public string Filter { get; set; }

        public ICollection<TriggerPerson> TriggerPersons { get; set; }
        public ICollection<TriggerReport> TriggerReports { get; set; }
        public ICollection<TriggerVolume> TriggerVolumes { get; set; }
    }
}
