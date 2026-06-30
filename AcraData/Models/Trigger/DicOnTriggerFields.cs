using System;
using System.Collections.Generic;

namespace AcraData.Models.Trigger
{
    public partial class DicOnTriggerFields
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string Description { get; set; }
    }
}
