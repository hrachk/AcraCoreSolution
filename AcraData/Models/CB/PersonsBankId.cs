using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class PersonsBankId
    {
        public long Id { get; set; }
        public long A002Id { get; set; }
        public long? BankId { get; set; }
        public string XmlReq { get; set; }
        public string XmlResp { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorText { get; set; }
        public string IdentityType { get; set; }
        public string ResidencyCountry { get; set; }
        public string Gender { get; set; }

        public A002 A002 { get; set; }
    }
}
