using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class P001
    {
        public long Id { get; set; }
        public string OrganisationCode { get; set; }
        public string OrganisationBranchCode { get; set; }
        public int? OrganizationStatus { get; set; }
        public bool OrganizationStatusSpecified { get; set; }
        public DateTime? SendDateTime { get; set; }

        public AcraAnswer AcraAnswer { get; set; }
    }
}
