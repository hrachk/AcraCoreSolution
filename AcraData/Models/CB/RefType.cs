using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class RefType
    {
        public RefType()
        {
            SourceReferences = new HashSet<SourceReference>();
        }

        public int RefTypeId { get; set; }
        public string RefTypeDesc { get; set; }

        public ICollection<SourceReference> SourceReferences { get; set; }
    }
}
