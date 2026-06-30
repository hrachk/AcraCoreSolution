using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class Person
    {
        public Person()
        {
            //Passports = new HashSet<Passport>();
            //IdCards = new HashSet<IdCard>();
        }

        [Key]
        [Column("PersonID", TypeName = "int(11) unsigned")]
        public uint PersonId { get; set; }
        [Column(TypeName = "int(11) unsigned")]
        public uint? FirstName { get; set; }
        [Column(TypeName = "int(11) unsigned")]
        public uint? LastName { get; set; }
        [StringLength(12)]
        public string PatronymicName { get; set; }
        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }
        [Column(TypeName = "int(11) unsigned")]
        public uint? Sex { get; set; }
        [StringLength(20)]
        public string SocialCard { get; set; }
        [Column("ResidentID")]
        public byte? ResidentId { get; set; }
        [Column("SourceID", TypeName = "int(11) unsigned")]
        public uint? SourceId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime IncomingDate { get; set; }
        //[Column(TypeName = "tinyint(3)")]
        //public sbyte BlockStatus { get; set; }

        //[InverseProperty("Person")]
        //public ICollection<Passport> Passports { get; set; }
        //[InverseProperty("Person")]
        //public ICollection<IdCard> IdCards { get; set; }
    }
}
