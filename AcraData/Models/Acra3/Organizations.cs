using System;
using System.Collections.Generic;

namespace AcraData.Models.Acra3
{
    public partial class Organization
    {
        public int OrganizationId { get; set; }
        public string Hvhh { get; set; }
        public int? OrganizationType { get; set; }
        public string StateRegistryNumber { get; set; }
        public int? OrgPropertyTypeId { get; set; }
        public int? ResidentId { get; set; }
        public int SourceId { get; set; }
        public DateTime IncomingDate { get; set; }      
        public DateTime FoundationDate { get; set; }
    }
}
