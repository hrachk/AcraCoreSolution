namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OrganizationOwners")]
    public partial class OrganizationOwner
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        public long? OrganizationID { get; set; }

        [StringLength(10)]
        public string ACRAID { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
