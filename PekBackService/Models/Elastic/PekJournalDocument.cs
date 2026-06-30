using System;

namespace PekBackService
{
    public class PekJournalDocument
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Request { get; set; }

        public object Response { get; set; }

        public long UserActivityId { get; set; }

        public string Status { get; set; }

        public int SourceId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}