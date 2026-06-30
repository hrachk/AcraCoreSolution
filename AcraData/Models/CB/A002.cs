using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class A002
    {
        public A002()
        {
            PersonsBankId = new HashSet<PersonsBankId>();
            RegPhotos = new HashSet<RegPhoto>();
            Registers = new HashSet<Register>();
        }

        public long Id { get; set; }
        public long PersonId { get; set; }
        public string IdentityNumber { get; set; }
        public int? Status { get; set; }
        public long? RegisterId { get; set; }
        public string XmlReq { get; set; }
        public string XmlResp { get; set; }
        public DateTime SendDateTime { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public Register Register { get; set; }
        public ICollection<PersonsBankId> PersonsBankId { get; set; }
        public ICollection<RegPhoto> RegPhotos { get; set; }
        public ICollection<Register> Registers { get; set; }
    }
}
