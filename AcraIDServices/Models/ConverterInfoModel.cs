using System;
using System.Collections.Generic;
using System.Text;

namespace AcraIDServices.Models
{
    public class ConverterPersonInfo
    {        
        public ConverterCheck<string> FirstName { get; set; }

        public ConverterCheck<string> LastName { get; set; }

        public ConverterCheck<string> SocialCard { get; set; }

        public ConverterCheck<List<string>> DocumentNum { get; set; }

        public ConverterCheck<string> Residency { get; set; }

        public ConverterCheck<string> BirthDate { get; set; }
    }

    public class ConverterCheck<T>
    {
        public bool Check { get; set; }
        public T Value { get; set; }
    }

    public class ConverterPersonInfoResponse
    {
        public string DocumentNum { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string SocialCard { get; set; }

        public string CertificateNumber { get; set; }

        public string Residency { get; set; }

        public string BirthDate { get; set; }
    }

    public class RequestModel
    {
        public string passport { get; set; }
    }

    public class RequestModelBySSN
    {
        public string SSN { get; set; }
    }
}
