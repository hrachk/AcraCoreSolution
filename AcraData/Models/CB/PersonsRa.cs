using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class PersonsRA
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public bool IsRa { get; set; }
        public int? Radelegate { get; set; }

        public Person Person { get; set; }
    }
}
