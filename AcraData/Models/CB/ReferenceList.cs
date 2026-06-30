using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class ReferenceList
    {
        public string RefName { get; set; }
        public string RefDescription { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
    }
}
