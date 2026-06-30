using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AcraData.Models.Acra3
{
    public partial class DeadPersons
    {
        [Key]
        [Column("id", TypeName = "int(11)")]
        public int? id { get; set; }
        [Column("request_code", TypeName = "int(11)")]
        public int? request_code { get; set; }
        [StringLength(100)]
        public string first_name { get; set; }
        [StringLength(100)]
        public string last_name { get; set; }
        [StringLength(100)]
        public string patronymic_name { get; set; }
        [StringLength(20)]
        public string social_card { get; set; }
        [StringLength(10)]
        public string document_type { get; set; }
        [StringLength(50)]
        public string document_num { get; set; }
        [Column(TypeName = "date")]
        public DateTime? birth_date { get; set; }
        [StringLength(10)]
        public string genus { get; set; }
        [Column(TypeName = "date")]
        public DateTime? reg_date { get; set; }
        [Column(TypeName = "date")]
        public DateTime? death_date { get; set; }
        [StringLength(20)]
        public string death_certificate_number { get; set; }
        [Column(TypeName = "date")]
        public DateTime? death_certificate_issuance_date { get; set; }
        [StringLength(20)]
        public string citizenship { get; set; }
        [StringLength(10)]
        public string nationality { get; set; }
        [StringLength(10)]
        public string country { get; set; }
        [Column(TypeName = "timestamp")]
        public DateTime? create_date { get; set; }
        [Column(TypeName = "timestamp")]
        public DateTime? modify_date { get; set; }
        [Column("modifier", TypeName = "int(1)")] 
        public int? modifier { get; set; }
    }
}
