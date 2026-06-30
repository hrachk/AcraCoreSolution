using AcraData.Models.Acra4;
using System;
using System.Collections.Generic;
using System.Text;

namespace AcraIDServices.Models
{
    public partial class ConverterRespModel
    {
        public int Status { get; set; } // OK = 0, Error = 1

        public List<ErrorType> ErrorTypes { get; set; }
    }
    public partial class NonResidentRespModel
    {
        public bool EkengStatus { get; set; }
        public bool IsValid { get; set; }
        public string Error { get; set; }
        public string SSN { get; set; }
        public List<BPR_Persons> Persons { get; set; }
    }

    //public partial class ConverterErrorModel
    //{
    //    public int ErrorCode { get; set; }

    //    public string ErrorMessage { get; set; }
    //}

    public enum ErrorType
    {
        SocialCard = 1,
        MatchingError = 2,
        Residency = 3,
        FirstName = 4,
        LastName = 5,
        BirthDate = 6,
        Ekeng = 7,
        MissingSocialCard = 8
    }
   
}
