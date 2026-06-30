namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("BPR_Addresses")]
    public partial class BPR_Addresses
    {
        [Key]
        [Column("ID", TypeName = "bigint(20)")]
        public long ID { get; set; }

        [ForeignKey("BPR_Persons")]
        public long AVVPersonID { get; set; }

        [StringLength(255)]
        public string LocationCode { get; set; }

        [StringLength(255)]
        public string Region { get; set; }

        [StringLength(255)]
        public string Community { get; set; }

        [StringLength(255)]
        public string Residence { get; set; }

        [StringLength(255)]
        public string Street { get; set; }

        [StringLength(255)]
        public string Building { get; set; }

        [StringLength(255)]
        public string BuildingType { get; set; }

        [StringLength(255)]
        public string Apartment { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime AVVGetDateTime { get; set; }

        public virtual BPR_Persons BPR_Persons { get; set; }

    }

}


//public partial class TriggerPersonsDetail
//{
//    public long Id { get; set; }
//    public long? Tpid { get; set; }
//    public int PersonId { get; set; }
//    public DateTime? SysDate { get; set; }

//    public TriggerPerson TriggerPerson { get; set; }
//}