using System;
using System.Collections.Generic;

namespace AcraData.Models.Acra3
{
    public partial class OrganizationNames
    {
        public int OrganizationNameId { get; set; }
        public int? OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public sbyte? ValidFlag { get; set; }
        public int? SourceId { get; set; }
        public DateTime? IncomingDate { get; set; }
    }
}
