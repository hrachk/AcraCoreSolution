namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ActualAddress")]
    public partial class ActualAddress
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

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

        [StringLength(50)]
        public string eMail { get; set; }

        [StringLength(20)]
        public string mobilePhone { get; set; }

        [Column(TypeName = "timestamp")]
        public DateTime ModifyDateTime { get; set; }
    }
}
