using System;
using System.Collections.Generic;

namespace AcraData.Models.CB
{
    public partial class AcraPerson
    {
        public AcraPerson()
        {
            PersonsBankIdreqs = new HashSet<PersonsBankIdReq>();
        }

        public int PersonId { get; set; }
        public int Status { get; set; }
        public long? BankId { get; set; }

        public ICollection<PersonsBankIdReq> PersonsBankIdreqs { get; set; }
    }
}
