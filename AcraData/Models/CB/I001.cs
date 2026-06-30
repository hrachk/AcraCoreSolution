using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class I001
    {
        public int Id { get; set; }
        public string AppName { get; set; }
        public string Data { get; set; }
        public string XmlResp { get; set; }
        public DateTime SendDateTime { get; set; }
    }
}
