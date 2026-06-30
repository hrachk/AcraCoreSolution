using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class ACRAID_MAPPER
    {
        public ACRAID_MAPPER()
        {
            //Passports = new HashSet<Passport>();
            //IdCards = new HashSet<IdCard>();
        }
        [Key]
        [Column("ID", TypeName = "int(11) unsigned")]
        public uint ID { get; set; }
        [Column(TypeName = "bigint(20)")]
        public long? ACRAID { get; set; }
        [Column(TypeName = "int(11)")]
        public uint? PersonID { get; set; }
        [Column(TypeName = "tinyint(3)")]
        public sbyte MatchingID { get; set; }
        [Column(TypeName = "date")]
        public DateTime? GenerationDate { get; set; }
       
    }
}
