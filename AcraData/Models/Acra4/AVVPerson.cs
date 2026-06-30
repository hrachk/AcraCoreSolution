namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;   

    [Table("BPR_Persons")]
    public partial class BPR_Persons
    {
        public BPR_Persons()
        {
            BPR_Documents = new HashSet<BPR_Documents>();          
        }
        [Key]
        public long ID { get; set; }
        
        public int? ACRAID { get; set; }

        [StringLength(50)]
        public string PNum { get; set; }

        [Column(TypeName = "bit")]
        public bool? SSNIndicator { get; set; }

        [StringLength(50)]
        public string CertificateNum { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsDead { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DeathDate { get; set; }

        [StringLength(255)]
        public string FirstName { get; set; }

        [StringLength(255)]
        public string LastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        [StringLength(1)]
        public string Gender { get; set; }       

        [Column(TypeName = "timestamp")]
        public DateTime? AVVGetDate { get; set; }
        
        public ICollection<BPR_Documents> BPR_Documents { get; set; }
        public BPR_Addresses BPR_Address { get; set; }      

        public virtual object Clone() { return this.MemberwiseClone(); }
    }
}
