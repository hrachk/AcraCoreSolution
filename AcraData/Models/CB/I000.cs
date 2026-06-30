using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class I000
    {
        public int Id { get; set; }
        public string AppName { get; set; }
        public string XmlResp { get; set; }
        public DateTime SendDateTime { get; set; }
    }
}
