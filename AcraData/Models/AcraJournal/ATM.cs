using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.AcraJournal
{
    public partial class ATM
    {       
        [Column("RequestID", TypeName = "bigint(20)")]
        public long RequestId { get; set; }        
        [StringLength(50)]
        public string SessionID { get; set; }
        public int ActivityId { get; set; }
        [StringLength(255)]
        public string Request { get; set; }
        [Column("xmlReq", TypeName = "text")]
        public string XmlReq { get; set; }
        [Column("xmlResp", TypeName = "text")]
        public string XmlResp { get; set; }
        [Column(TypeName = "int(11)")]
        public int? ErrorCode { get; set; }
        [Column(TypeName = "timestamp")]
        public DateTimeOffset ReqDateTime { get; set; }       
    }
}
