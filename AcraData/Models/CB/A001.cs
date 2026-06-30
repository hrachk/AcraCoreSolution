using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class A001
    {
        public A001()
        {
            AcraAnswersDouble = new HashSet<AcraAnswerDouble>();
            AcraAnswers = new HashSet<AcraAnswer>();
        }

        public long Id { get; set; }
        public long? ReqId { get; set; }
        public string XmlReq { get; set; }
        public string XmlResp { get; set; }
        public int? ReqCount { get; set; }
        public int? RespCount { get; set; }
        public DateTime SendDateTime { get; set; }
        public long? LastReqId { get; set; }

        public ICollection<AcraAnswerDouble> AcraAnswersDouble { get; set; }
        public ICollection<AcraAnswer> AcraAnswers { get; set; }
    }
}
