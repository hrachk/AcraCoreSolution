using AcraData.Data;
using AcraData.Models.Acra4;
using AcraIDServices.Mappers;
using AcraUtils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http.Headers;
using AcraData.Models.Acra3;
using System.Reflection;

namespace AcraIDServices.Models
{
    public abstract class Person_AcraIdentityValidatorModel : IAcraIdentityValidatorModel
    {
        AVVMapper _avvMapper;
        private Logger _logger;
        DbContextOptions<Acra3DbContext> _acra3DbOptions;
        DbContextOptions<Acra4DbContext> _acra4DbOptions;

        dynamic responseModel;

        public Person_AcraIdentityValidatorModel(DbContextOptions<Acra3DbContext> acra3dbOptions, DbContextOptions<Acra4DbContext> acra4dbOptions, Logger logger)
        {

            _acra3DbOptions = acra3dbOptions;
            _acra4DbOptions = acra4dbOptions;
            _logger = logger;
            _avvMapper = new AVVMapper(_logger, acra4dbOptions);
        }

        public abstract int? AcraIdentityService(int EntityID);
        public abstract int? AcraIdentityValidator(int EntityID);       
        public abstract bool CheckAllDocsExistance(string IDNum, int EntityID);
        public abstract int? ComputeACRAID(string IDNum, int EntityID);
        public abstract int GenerateACRAID(string IDNum);
        public abstract dynamic Get3rdSourceIDNum(string Document);
        public abstract void Get3rdSourceInfo(string IDNum, string url);
        public abstract string GetIDNumFrom3rdSource(int EntityID);
        public abstract string GetIDNumFrom3rdSource(string IDNum);
        public abstract string GetIDNumFromACRA(int EntityID);
        public abstract List<string> GetPersonsDocs(int EntityID);
        public abstract bool IDNumExistanceIn3rdSource(string IDNum);
        public abstract bool Is3rdSourceUpToDate(string IDNum);
        public abstract bool IsPrevDataChanged(dynamic person);
        public abstract void Log3rdSourceRequests(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage);
    }

    public abstract class Legal_AcraIdentityValidatorModel : IAcraIdentityValidatorModel
    {
        AVVMapper _avvMapper;
        private Logger _logger;
        DbContextOptions<Acra3DbContext> _acra3DbOptions;
        DbContextOptions<Acra4DbContext> _acra4DbOptions;

        dynamic responseModel;

        public Legal_AcraIdentityValidatorModel(DbContextOptions<Acra3DbContext> acra3dbOptions, DbContextOptions<Acra4DbContext> acra4dbOptions, Logger logger)
        {

            _acra3DbOptions = acra3dbOptions;
            _acra4DbOptions = acra4dbOptions;
            _logger = logger;
            _avvMapper = new AVVMapper(_logger, acra4dbOptions);
        }

        public abstract int? AcraIdentityService(int EntityID);
        public abstract int? AcraIdentityValidator(int EntityID);       
        public abstract bool CheckAllDocsExistance(string IDNum, int EntityID);
        public abstract int? ComputeACRAID(string IDNum, int EntityID);
        public abstract int GenerateACRAID(string IDNum);
        public abstract dynamic Get3rdSourceIDNum(string Document);
        public abstract void Get3rdSourceInfo(string IDNum, string url);
        public abstract string GetIDNumFrom3rdSource(int EntityID);
        public abstract string GetIDNumFrom3rdSource(string IDNum);
        public abstract string GetIDNumFromACRA(int EntityID);
        public abstract List<string> GetPersonsDocs(int EntityID);
        public abstract bool IDNumExistanceIn3rdSource(string IDNum);
        public abstract bool Is3rdSourceUpToDate(string IDNum);
        public abstract bool IsPrevDataChanged(dynamic person);
        public abstract void Log3rdSourceRequests(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage);
    }
}
