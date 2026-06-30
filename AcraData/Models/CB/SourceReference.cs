using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class SourceReference
    {
        public long Id { get; set; }
        public int RefTypeId { get; set; }
        public long RefId { get; set; }
        public long SourceId { get; set; }
        public int EntityTypeId { get; set; }
        public long EntityId { get; set; }

        public Entity EntityType { get; set; }
        public RefType RefType { get; set; }
    }
}
