using System;
using System.Collections.Generic;

namespace AcraData.Models.Acra3
{
    public partial class Source
    {
        public int SourceId { get; set; }
        public string SourceName { get; set; }
        public string Xmlname { get; set; }
        public int? SourceType { get; set; }
        public sbyte CreditorTypeId { get; set; }
        public string CreditorCode { get; set; }
        public string ShortName { get; set; }
        public bool? ShowInReport { get; set; }
        public string HomePage { get; set; }
        public string Fax { get; set; }
        public string EMail { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string AccountNumber { get; set; }
        public string Bank { get; set; }
        public string Hvhh { get; set; }
        public string Manager { get; set; }
        public string Accountant { get; set; }
        public int? SpecialDiscount { get; set; }
        public string ContractId { get; set; }
        public DateTime? ContractDate { get; set; }
        public string GenerateAcraID { get; set; }
    }
}
