using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class Entity
    {
        public Entity()
        {
            SourceReferences = new HashSet<SourceReference>();
        }

        public int EntityTypeId { get; set; }
        public string EntityDesc { get; set; }

        public ICollection<SourceReference> SourceReferences { get; set; }
    }
}
