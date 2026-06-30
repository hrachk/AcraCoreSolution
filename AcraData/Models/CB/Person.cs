using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class Person
    {
        public Person()
        {
            Addresses = new HashSet<Address>();
            FullAntps = new HashSet<FullAntp>();
            Identities = new HashSet<Identity>();
            PersonsFC = new HashSet<PersonsFC>();
            PersonsFP = new HashSet<PersonsFP>();
            PersonsLE = new HashSet<PersonsLE>();
            PersonsRA = new HashSet<PersonsRA>();
        }

        public long Id { get; set; }
        public long? RefId { get; set; }
        public int SystemStatus { get; set; }
        public DateTime? SystemDate { get; set; }
        public string BankId { get; set; }
        public string Notes { get; set; }

        public ICollection<Address> Addresses { get; set; }
        public ICollection<FullAntp> FullAntps { get; set; }
        public ICollection<Identity> Identities { get; set; }
        public ICollection<PersonsFC> PersonsFC { get; set; }
        public ICollection<PersonsFP> PersonsFP { get; set; }
        public ICollection<PersonsLE> PersonsLE { get; set; }
        public ICollection<PersonsRA> PersonsRA { get; set; }
    }
}
