using System;
using System.Collections.Generic;

namespace AcraData.Models.Trigger
{
    public partial class TriggerVolume
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Tsid { get; set; }
        public int? Volume { get; set; }

        public TriggerSource Ts { get; set; }
    }
}
