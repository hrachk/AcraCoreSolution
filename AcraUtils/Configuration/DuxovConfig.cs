using System;
using System.Collections.Generic;
using System.Text;

namespace AcraUtils.Configuration
{
    public class DuxovConfig
    {        
        public decimal Amount { get; set; }
        public string CurrencyTable { get; set; }
        public DateTime MaxIncomingDate { get; set; }
        public int ProcessCount { get; set; }
    }
}
