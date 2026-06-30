using System;
using System.Collections.Generic;

namespace AcraData.Models.Trigger
{
    public partial class TriggerReport
    {
        public long Id { get; set; }
        public int? Tsid { get; set; }
        public int SourceId { get; set; }
        public int PersonId { get; set; }
        public int? ReportId { get; set; }
        public int? ReasonId { get; set; }
        public int? SubReasonId { get; set; }
        public long? UserActivityId { get; set; }
        public DateTime ActivityTime { get; set; }
        public string ReportInfo { get; set; }
        public DateTime? SysDate { get; set; }

        public TriggerSource TriggerSource { get; set; }
    }
}
