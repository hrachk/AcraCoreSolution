using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class AcraAnswerDouble
    {
        public long Id { get; set; }
        public long ReqId { get; set; }
        public string SenderOrgCode { get; set; }
        public string DestinationOrgCode { get; set; }
        public string AppName { get; set; }
        public string DocType { get; set; }
        public string XmlReq { get; set; }
        public string XmlResp { get; set; }
        public int SystemStatus { get; set; }
        public DateTime SystemDate { get; set; }
        public int? SystemErrorCode { get; set; }
        public string SystemErrorDesc { get; set; }
        public long? A001id { get; set; }
        public DateTime? ResponseDate { get; set; }

        public A001 A001 { get; set; }
    }
}
