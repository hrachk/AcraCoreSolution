namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
   

    [Table("ACRAIdentity")]
    public partial class ACRAIdentity
    {
        [Key]
        public int ACRAID { get; set; }

       
        public string ACRAGroup { get; set; }

        [Column(TypeName = "bit")]
        public bool? IsLegal { get; set; }
              
    }
}
