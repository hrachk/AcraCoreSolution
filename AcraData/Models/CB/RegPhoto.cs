using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class RegPhoto
    {
        public long Id { get; set; }
        public long A002Id { get; set; }
        public string SocCard { get; set; }
        public byte[] Photo { get; set; }

        public A002 A002 { get; set; }
    }
}
