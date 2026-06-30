using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.ATM
{
    public partial class ActivityLog
    {
        public ActivityLog()
        {
            ActivityOrgs = new HashSet<ActivityOrg>();
            ActivityPersons = new HashSet<ActivityPerson>();
            ActivitySearchOrgs = new HashSet<ActivitySearchOrg>();
            ActivitySearchPersons = new HashSet<ActivitySearchPerson>();
            RepRequests = new HashSet<RepRequest>();
        }

        [Key]
        [Column("ActivityID", TypeName = "bigint(20)")]
        public long ActivityId { get; set; }
        [Column("SessionID")]
        [StringLength(100)]
        public string SessionId { get; set; }
        [Column("AppID", TypeName = "int(11)")]
        public int? AppId { get; set; }
        [Column("SourceID", TypeName = "int(11)")]
        public int? SourceId { get; set; }
        [Column("UserID", TypeName = "int(11)")]
        public int? UserId { get; set; }
        [Column("XMLRequest", TypeName = "text")]
        public string XmlRequest { get; set; }
        [Column("XMLResponse", TypeName = "text")]
        public string XmlResponse { get; set; }
        [Column("XMLReported", TypeName = "text")]
        public string XmlReported { get; set; }
        [Column(TypeName = "bit(1)")]
        public bool? IsReported { get; set; }
        [StringLength(255)]
        public string ErrorCode { get; set; }
        [StringLength(255)]
        public string Description { get; set; }
        [Column(TypeName = "timestamp")]
        public DateTimeOffset? RequestDateTime { get; set; }
        [Column(TypeName = "timestamp")]
        public DateTimeOffset? ResponseDateTime { get; set; }
        [Column(TypeName = "timestamp")]
        public DateTimeOffset? ReportedDateTime { get; set; }

        [InverseProperty("Activity")]
        public ActivityLogDetail ActivityLogDetail { get; set; }
        [InverseProperty("Activity")]
        public ActivitySearchOrgParam ActivitySearchOrgParam { get; set; }
        [InverseProperty("Activity")]
        public ActivitySearchPersonParam ActivitySearchPersonParam { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivityOrg> ActivityOrgs { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivityPerson> ActivityPersons { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivitySearchOrg> ActivitySearchOrgs { get; set; }
        [InverseProperty("Activity")]
        public ICollection<ActivitySearchPerson> ActivitySearchPersons { get; set; }
        [InverseProperty("Activity")]
        public ICollection<RepRequest> RepRequests { get; set; }
    }
}
