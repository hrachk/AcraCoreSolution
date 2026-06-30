using AcraData.Models.Acra3;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AcraIDServices.Models
{
    interface IAcraIdentityValidatorModel
    {       
        int? AcraIdentityValidator(int EntityID);
        int? AcraIdentityService(int EntityID);
        string GetIDNumFrom3rdSource(int EntityID);
        string GetIDNumFrom3rdSource(string IDNum);
        string GetIDNumFromACRA(int EntityID);
        bool IDNumExistanceIn3rdSource(string IDNum);
        bool CheckAllDocsExistance(string IDNum, int EntityID);
        bool Is3rdSourceUpToDate(string IDNum);
        List<string> GetPersonsDocs(int EntityID);
        bool IsPrevDataChanged(dynamic person);
        int? ComputeACRAID(string IDNum, int EntityID);
        int GenerateACRAID(string IDNum);
        void Get3rdSourceInfo(string IDNum, string url);
        void Log3rdSourceRequests(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage);
        dynamic Get3rdSourceIDNum(string Document);
    }    
}
