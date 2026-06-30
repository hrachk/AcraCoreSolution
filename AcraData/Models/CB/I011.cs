using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class I011
    {
        public int Id { get; set; }
        public string RefName { get; set; }
        public string XmlResp { get; set; }
        public int? ParseStatus { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime SendDateTime { get; set; }
    }
}
