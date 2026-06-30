using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class ScorePerson
    {
        [Key]
        [Column("PersonID", TypeName = "int(11)")]
        public int PersonID { get; set; }       
        [Column("ScorePersonID", TypeName = "int(11)")]
        public int ScorePersonID { get; set; }
        [Column(TypeName = "date")]
        public DateTime? IncomingDate { get; set; }
        public sbyte Status { get; set; }    
    }
}
