using System;
using System.Collections.Generic;
using System.Text;

namespace AcraUtils.Configuration
{
    public class PekConfig
    {
        public string PekBackUrl { get; set; }
        public string SendErrorsFromEmail { get; set; }
        public string SendErrorsToEmail { get; set; }
        public string SMTPClient { get; set; }
        public string Endpoint { get; set; }
    }
}
