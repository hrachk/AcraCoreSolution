using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcraData.Models.Acra3
{
    public partial class OrgOwner
    {
        public long OrgOwnerID { get; set; }
        public Nullable<int> OrganizationID { get; set; }
        public Nullable<int> PersonID { get; set; }
        public Nullable<long> OwnerOrgID { get; set; }
        public Nullable<int> ParticipationType { get; set; }
        public Nullable<int> SourceID { get; set; }
        public System.DateTime IncomingDate { get; set; }
        public Nullable<long> ReceivedPackageID { get; set; }
    }
}
