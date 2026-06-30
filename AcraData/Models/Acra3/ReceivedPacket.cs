using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class ReceivedPacket
    {
        [Key]
        public long ReceivedPackageID { get; set; }
        public Nullable<int> UserID { get; set; }
        public Nullable<int> SourceID { get; set; }
        public Nullable<System.DateTime> IncomingDate { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public string ExternalPackageID { get; set; }
        public Nullable<byte> FileCount { get; set; }
        public Nullable<sbyte> PackageStatus { get; set; }
        public byte ConvertStatus { get; set; }
        public Nullable<System.DateTime> StatusModifyDate { get; set; }
        public Nullable<System.DateTime> ProcessStart { get; set; }
        public Nullable<System.DateTime> ProcessEnd { get; set; }
    }
}
