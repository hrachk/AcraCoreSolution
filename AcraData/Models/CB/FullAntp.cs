using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class FullAntp
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public string Antp { get; set; }
        public string RegNum { get; set; }
        public string ActivityField { get; set; }
        public DateTime? RegDate { get; set; }

        public Person Person { get; set; }
    }
}
