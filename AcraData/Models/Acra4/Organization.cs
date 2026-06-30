namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Organization")]
    public partial class Organization
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Organization()
        {
            OrganizationOwners = new HashSet<OrganizationOwner>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [StringLength(10)]
        public string ACRAID { get; set; }

        [StringLength(255)]
        public string TIN { get; set; }

        [StringLength(255)]
        public string RegNum { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RegDate { get; set; }

        public int? OrganizationType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrganizationOwner> OrganizationOwners { get; set; }
    }
}
