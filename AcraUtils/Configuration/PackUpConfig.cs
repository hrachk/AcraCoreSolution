using System;
using System.Collections.Generic;
using System.Text;

namespace AcraUtils.Configuration
{
    public class PackUpConfig
    {
        public string[] Destination { get; set; }
        public string CheckUpBackEndURL { get; set; }
        public bool Switch { get; set; }
        public string TimeOfReopening { get; set; }
        public string VersionControl { get; set; }
    }

    public class ValidatorConfig
    {
        public string EkengServiceURL { get; set; }
    }
    public class AcraIDGeneratorConfig
    {
        public string EkengServiceURL { get; set; }
        public string SendErrorsFromEmail { get; set; }
        public string SendErrorsToEmail { get; set; }
        public string SMTPClient { get; set; }
        public int ThreadCount { get; set; }
    }
}
