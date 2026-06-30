using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class PackageFile
    {
        [Key]
        public long PackageFileID { get; set; }
        public Nullable<long> ReceivedPackageID { get; set; }
        public Nullable<long> SourceID { get; set; }
        public Nullable<System.DateTime> ReceivedDate { get; set; }
        public string ExternalPackageID { get; set; }
        public string FileName { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public Nullable<byte> FileCount { get; set; }
        public Nullable<byte> FileNum { get; set; }
        public Nullable<byte> FileStatus { get; set; }
    }
}
