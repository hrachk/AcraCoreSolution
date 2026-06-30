using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class AcraAnswer
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

        public L001 L001 { get; set; }
        public L002 L002 { get; set; }
        public L003 L003 { get; set; }
        public L004 L004 { get; set; }
        public P001 P001 { get; set; }
    }
}
