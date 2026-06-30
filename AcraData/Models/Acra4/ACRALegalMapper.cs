namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    //[Table("ACRALegalMapper")]
    //public partial class ACRALegalMapper
    //{
    //    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    //    public long ID { get; set; }

    //    [Required]
    //    [StringLength(10)]
    //    public string ACRAID { get; set; }

    //    public long OrganizationID { get; set; }
    //}

    public partial class ACRALegalMapper
    {
        [Key]
        [Column("OrganizationID", TypeName = "int(11)")]
        public int OrganizationID { get; set; }
        [Column("ACRAID", TypeName = "int(11)")]
        public int ACRAID { get; set; }
        public string BANKID { get; set; }
        public int? StageID { get; set; }
        [Column(TypeName = "date")]
        public DateTime? IncomingDate { get; set; }
        public int? Status { get; set; }
    }
}
