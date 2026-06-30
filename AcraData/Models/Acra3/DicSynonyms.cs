using System;
using System.Collections.Generic;

namespace AcraData.Models.Acra3
{
    public partial class DicSynonym
    {
        public int ID { get; set; }
        public int SourceID { get; set; }
        public string Type { get; set; }
        public string AcraValue { get; set; }
        public string BankValue { get; set; }       
    }
}
