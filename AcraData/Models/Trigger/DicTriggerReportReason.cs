using System;
using System.Collections.Generic;

namespace AcraData.Models.Trigger
{
    public partial class DicTriggerReportReason
    {
        public int Id { get; set; }
        public int? ReportReasonId { get; set; }
        public int? ReportSubReasonId { get; set; }
        public int SourceId { get; set; }
    }
}
