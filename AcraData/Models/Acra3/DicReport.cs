using System;
using System.Collections.Generic;

namespace AcraData.Models.Acra3
{
    public partial class DicReport
    {
        public int ReportId { get; set; }
        public string Report { get; set; }
        public int ReportType { get; set; }
        public int? ReportPrice { get; set; }
        public int? ScoreReport { get; set; }
    }
}
