namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Text.Json.Serialization;

    [Table("BPR_Documents")]
    public partial class BPR_Documents
    {
       
        [Key]
        [Column("ID", TypeName = "bigint(20)")]
        public long ID { get; set; }

        public long AVVPersonID { get; set; }
        public string Photo { get; set; }
        public int DocumentType { get; set; }

        [StringLength(255)]
        public string DocumentNumber { get; set; } 
        
        [StringLength(50)]
        public string DocumentStatus { get; set; }

        [StringLength(255)]
        public string DocumentDepartment { get; set; }

        [StringLength(255)]
        public string CountryName { get; set; }

        [StringLength(10)]
        public string CountryCode { get; set; }       

        [Column(TypeName = "date")]
        public DateTime? IssuanceDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ValidityDate { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string MiddleName { get; set; }

        [StringLength(50)]
        public string EnglishLastName { get; set; }

        [StringLength(50)]
        public string EnglishFirstName { get; set; }

        [StringLength(50)]
        public string EnglishMiddleName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        [StringLength(1)]
        public int Gender { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime AVVGetDateTime { get; set; }

        [JsonIgnore]
        public BPR_Persons BPR_Persons { get; set; }
    }
}
