using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class Address
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public int? TypeId { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Street { get; set; }
        public string BuildNumber { get; set; }
        public string Appartment { get; set; }

        public Person Person { get; set; }
    }
}
