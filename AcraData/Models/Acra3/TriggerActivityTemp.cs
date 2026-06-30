using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AcraData.Models.Acra3
{
    public partial class TriggerActivityTemp
    {
        [Key]
        public long Id { get; set; }
        public long ActivityId { get; set; }
        public Nullable<int> ActivityType { get; set; }
        public int Status { get; set; }
    }    
}
