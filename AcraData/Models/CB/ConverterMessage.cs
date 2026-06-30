using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class ConverterMessage
    {
        public long Id { get; set; }
        public string RawData { get; set; }
        public DateTime InsertDate { get; set; }
        public int Status { get; set; }
    }
}
