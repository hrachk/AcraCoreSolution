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

namespace AcraIDServices
{
    public class CollectAVVInfoModel
    {
        public static List<Models.PersonInfo> PersonsInfos = new List<Models.PersonInfo>();

        AVVMapper _avvMapper;
        private Logger _logger;

        DbContextOptions<Acra3DbContext> _acra3DbOptions;
        DbContextOptions<Acra4DbContext> _acra4DbOptions;
        DbContextOptions<AcraJournalDbContext> _acraJournalOptions;

        private Models.AVV.AvvResponse responseModel = new Models.AVV.AvvResponse();       

        public CollectAVVInfoModel(DbContextOptions<Acra3DbContext> acra3dbOptions, DbContextOptions<Acra4DbContext> acra4dbOptions, Logger logger, DbContextOptions<AcraJournalDbContext> acraJournalOptions)
        {

            _acra3DbOptions = acra3dbOptions;
            _acra4DbOptions = acra4dbOptions;
            _acraJournalOptions = acraJournalOptions;
            _logger = logger;
            _avvMapper = new AVVMapper(_logger, acra4dbOptions);
        }

        public void Activity(TriggerActivityTemp activityItem)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            using (var DB = new Acra3DbContext(_acra3DbOptions))
            {                
                DB.ChangeTracker.AutoDetectChangesEnabled = false;
                using (var tx = DB.Database.BeginTransaction())
                {                    
                    try
                    {
                        int? ACRAID = null;
                        switch (activityItem.ActivityType)
                        {
                            case 11:
                                ACRAID = CollectInfo((int)activityItem.ActivityId);
                                break;
                            default:
                                break;
                        }
                        /* DELETE
                        //DB.LoanActivityTmps.Attach(activityItem);
                        //DB.LoanActivityTmps.Remove(activityItem);
                        */
                        activityItem.Status = (ACRAID == null)?100:200;
                        DB.Entry(activityItem).State = EntityState.Detached;
                        DB.TriggerActivityTmps.Update(activityItem);
                        DB.SaveChanges();
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        activityItem.Status = 2;
                        DB.Entry(activityItem).State = EntityState.Detached;
                        DB.TriggerActivityTmps.Update(activityItem);
                        _logger.Log.ErrorFormat("CollectAVVInfo  Activity failed CreditId:{0} Error:{1}", activityItem.ActivityId, ex.Message);
                        throw (new Exception("CollectAVVInfo  failed", ex));
                    }
                    finally
                    {
                        DB.SaveChanges();
                    }
                }
            }

            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
        }


        private int? CollectInfo(int PersonID)
        {
            int? result = null;
            var personAcraInfos = PersonsInfos.Where(p => p.PersonId == PersonID).ToList();
            // 1. Check SSN
            if (!string.IsNullOrEmpty(personAcraInfos.First().SocialCard))
            {
                if (!SSNExistanceIn3rdSource(personAcraInfos.First().SocialCard))
                {
                    responseModel = null;
                    Get3rdSourceInfoBySSN(personAcraInfos.First().SocialCard);

                    if (responseModel!= null && responseModel.Result != null)
                    {
                        if (responseModel.Result.Count == 1)
                            if (_avvMapper.ImportPerson(responseModel.Result.FirstOrDefault()) != null)
                                result = 1;
                    }                  
                }
                else
                    result = 1;
            }
            

            //2. Check All docs
            foreach (var personAcraDoc in personAcraInfos)
            {
                if (!DocExistanceIn3rdSource(personAcraDoc.DocumentNum))
                {
                    responseModel = null;
                    Get3rdSourceInfoByDoc(personAcraDoc.DocumentNum);
                    if (responseModel != null && responseModel.Result != null)
                    {
                        if (responseModel.Result.Count == 1)
                            if (_avvMapper.ImportPerson(responseModel.Result.FirstOrDefault()) != null)
                                result = 1;
                    }
                }
                else
                    result = 1;
            }

            return result;
        }

        public bool SSNExistanceIn3rdSource(string SSN)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using var context = new Acra4DbContext(_acra4DbOptions);

            // используем FirstOrDefault + проверку на null, или Any()
            return context.BPR_Persons.Any(p => p.PNum == SSN || p.CertificateNum == SSN);
        }

        public bool DocExistanceIn3rdSource(string Document)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using var context = new Acra4DbContext(_acra4DbOptions);

            return context.BPR_Documents.Any(p => p.DocumentNumber == Document);
        }


        private void Get3rdSourceInfoBySSN(string SSN)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            Models.AVV.BySSN personData = new Models.AVV.BySSN { psn = SSN, Addresses = Models.AVV.Addresses.CURRENT };
            var url = "http://localhost:9070/AVV/GetPersonInfoBySSN";

            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 1, 0);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(JsonConvert.SerializeObject(personData), Encoding.UTF8, "application/json") };
            HttpResponseMessage response = client.SendAsync(request).Result;

            Log3rdSourceRequests(request, response);
            if (response.IsSuccessStatusCode)
            {
                responseModel = JsonConvert.DeserializeObject<Models.AVV.AvvResponse>(response.Content.ReadAsStringAsync().Result);               
            }
            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
        }      
      
        private void Get3rdSourceInfoByDoc(string Document)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            Models.AVV.ByDocument personData = new Models.AVV.ByDocument { docnum = Document, Addresses = Models.AVV.Addresses.CURRENT };
            var url = "http://localhost:9070/AVV/GetPersonInfoByDocument";

            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 1, 0);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(JsonConvert.SerializeObject(personData), Encoding.UTF8, "application/json") };            
            HttpResponseMessage response = client.SendAsync(request).Result;
            Log3rdSourceRequests(request, response);

            if (response.IsSuccessStatusCode)
            {
                responseModel = JsonConvert.DeserializeObject<Models.AVV.AvvResponse>(response.Content.ReadAsStringAsync().Result);
            }
            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");            
        }

        private void Log3rdSourceRequests(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            using (var context = new AcraData.Data.AcraJournalDbContext(_acraJournalOptions))
            {
                try
                {
                    context.BPR_Transaction .Add(new BPR_Transaction() { Request = requestMessage.Content.ReadAsStringAsync().Result, Response = responseMessage.Content.ReadAsStringAsync().Result, ResponseDateTime = DateTime.Now });
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Log.Error($"Error Method: {MethodBase.GetCurrentMethod().Name} Error:{ex.Message}");
                }
            }
            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
        }

    }
}
