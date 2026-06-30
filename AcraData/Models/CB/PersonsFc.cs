using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class PersonsFC
    {
        public long Id { get; set; }
        public long PersonId { get; set; }
        public string Country { get; set; }
        public int Fcdelegate { get; set; }
        public string Fcname { get; set; }

        public Person Person { get; set; }
    }
}
