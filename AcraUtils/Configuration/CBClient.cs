using System;
using System.Collections.Generic;
using System.Text;

namespace AcraUtils.Configuration
{
    public class CBClient
    {
        public string OrganisationCode { get; set; }
        public string OrganisationBranchCode { get; set; }
        public int QueryCount { get; set; }
        public int MaxAttempts { get; set; }
        public int AttemptsTimeout { get; set; }
    }
}
