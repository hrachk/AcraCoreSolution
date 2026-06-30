namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;   

    [Table("ACRAPersonMapperActivity")]
    public partial class ACRAPersonMapperActivity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Required]        
        public int ACRAID { get; set; }

        public long PersonID { get; set; }

        public string BANKID { get; set; }

        [Column(TypeName = "bit")]
        public bool isRemoved { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime ActionDate { get; set; }
    }
}
