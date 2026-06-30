namespace AcraData.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("BPR_DocumentTypes")]
    public partial class BPR_DocumentTypes
    {
        [Key]
        public int ID { get; set; }
       
        public string DocumentType { get; set; }       
    }
}


/*namespace ACRA3.Data.Models.Acra4
{
    using System;
    using System.Collections.Generic;
    
    public partial class DocumentType
    {
        public int ID { get; set; }
        public string DocumentType1 { get; set; }
    }
}
*/
