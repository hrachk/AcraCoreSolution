using System;

namespace CheckUpBackEndService
{
    public class CheckUpJournalDocument
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>RegisterPackage / RestrictPermissions / GetSource / GetIsMemberOrg</summary>
        public string EventType { get; set; }

        public string UserName { get; set; }

        public string SourceName { get; set; }

        public string SessionId { get; set; }

        public string FileName { get; set; }

        public string Thumbprint { get; set; }

        /// <summary>ErrorCode из Response</summary>
        public int ErrorCode { get; set; }

        public string ErrorDesc { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
