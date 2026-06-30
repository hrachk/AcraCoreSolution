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
    public class AcraIdentityValidatorEkengModel
    {
        AVVMapper _avvMapper;
        private Logger _logger;
        DbContextOptions<Acra3DbContext> _acra3DbOptions;
        DbContextOptions<Acra4DbContext> _acra4DbOptions;
        DbContextOptions<AcraJournalDbContext> _acraJournalOptions;

        private Models.PDataModel responseModel = new Models.PDataModel();       

        public AcraIdentityValidatorEkengModel(DbContextOptions<Acra3DbContext> acra3dbOptions, DbContextOptions<Acra4DbContext> acra4dbOptions, Logger logger, DbContextOptions<AcraJournalDbContext> acraJournalOptions)
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
                            case 6:
                                ACRAID = AcraIdentityService((int)activityItem.ActivityId);
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
                        _logger.Log.ErrorFormat("ACRAID Validataor Activity failed CreditId:{0} Error:{1}", activityItem.ActivityId, ex.Message);
                        throw (new Exception("ACRAID Validataor failed", ex));
                    }
                    finally
                    {
                        DB.SaveChanges();
                    }
                }
            }

            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
        }

        public int? AcraIdentityValidator(string SSN, int PersonID)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            if (SSNExistanceIn3rdSource(SSN))
            {
                SSN = GetSSNFrom3rdSource(SSN);
                if (CheckAllDocsExistance(SSN, PersonID))
                {
                    return ComputeACRAID(SSN, PersonID);
                }
                else
                {
                    if (Is3rdSourceUpToDate(SSN))
                    {
                        return null;
                    }
                    else
                    {
                        Get3rdSourceInfo(SSN);
                        if (responseModel.PassportData == null)
                            return null;
                        if (IsPrevDataChanged(responseModel))
                            ACRAIDCleaner(SSN);
                        _avvMapper.ImportPerson(responseModel);
                        return AcraIdentityValidator(SSN, PersonID);
                    }
                }
            }
            else
            {
                Get3rdSourceInfo(SSN);
                if (responseModel.PassportData == null)
                    return null;
                if (IsPrevDataChanged(responseModel))
                    ACRAIDCleaner(SSN);
                _avvMapper.ImportPerson(responseModel);
                return AcraIdentityValidator(SSN, PersonID);
            }
        }

        public int? AcraIdentityService(int PersonID)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            string SSN = GetSSNFromACRA(PersonID);
            int? ACRAID = null;
            if (!string.IsNullOrEmpty(SSN))
                return AcraIdentityValidator(SSN, PersonID);
            else
            {
                SSN = GetSSNFrom3rdSource(PersonID);
                if (!string.IsNullOrEmpty(SSN))
                    return ACRAID = AcraIdentityValidator(SSN, PersonID);
                else
                    return null;
            }              
        }

        public string GetSSNFrom3rdSource(int PersonID)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            List<string> personDocs = GetPersonsDocs(PersonID);            
            bool isSSNOK = true;
            int i = 0;
            string SSN = string.Empty;
            while (isSSNOK && i < personDocs.Count)
            {
                Models.PNumModel pNum = Get3rdSourceSSN(personDocs[i]);
                if (pNum != null)
                {
                    if (i > 0)                    
                        isSSNOK = SSN.Equals(pNum.data.PNum.Trim());                    
                    else
                        SSN = pNum.data.PNum.Trim();
                }
                i++;
            }
            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
            return (isSSNOK) ? SSN : string.Empty;           
        }
        
        public string GetSSNFrom3rdSource(string SSN)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using (Acra4DbContext context = new Acra4DbContext(_acra4DbOptions))
            {
                return System.Linq.Queryable.Where(context.BPR_Persons, p => p.PNum == SSN || p.CertificateNum == SSN).FirstOrDefault().PNum;
            }
        }

        public string GetSSNFromACRA(int PersonID)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using (Acra3DbContext context = new Acra3DbContext(_acra3DbOptions))
            {
               return System.Linq.Queryable.Where(context.Persons, p => p.PersonId == PersonID).FirstOrDefault()?.SocialCard??string.Empty;
            }
        }

        public bool SSNExistanceIn3rdSource(string SSN)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using (Acra4DbContext context = new Acra4DbContext(_acra4DbOptions))
            {
                return System.Linq.Queryable.Where(context.BPR_Persons, p => p.PNum == SSN || p.CertificateNum == SSN).Count() > 0;
            }
        }

        public bool CheckAllDocsExistance(string SSN, int PersonID)
        {
            using (Acra4DbContext context = new Acra4DbContext(_acra4DbOptions))
            {
                _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
                var avvDocuments = from d in context.BPR_Documents
                                   join p in context.BPR_Persons on d.AVVPersonID equals p.ID
                                   where p.PNum == SSN
                                   select new { d.DocumentNumber };

                List<string> docsList = GetPersonsDocs(PersonID);

                bool allDocsExistInAvv = true;
                foreach (var document in docsList)
                {
                    allDocsExistInAvv &= (avvDocuments.Where(d => d.DocumentNumber == document).Count() > 0);
                }
                _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
                return allDocsExistInAvv;
            }
        }

        public bool Is3rdSourceUpToDate(string SSN)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using (Acra4DbContext context = new Acra4DbContext(_acra4DbOptions))
            {
                return System.Linq.Queryable.Where(context.BPR_Persons, p => p.PNum == SSN).First().AVVGetDate.Value.Date == DateTime.Now.Date;
            }
        }

        private List<string> GetPersonsDocs(int personID)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            List<string> docsList = new List<string>();
            using (Acra3DbContext Acra3DB = new Acra3DbContext(_acra3DbOptions))
            {
                List<AcraData.Models.Acra3.Predicat> personsList = new List<AcraData.Models.Acra3.Predicat>();

                try
                {
                    string sql = string.Format(@"SELECT Distinct IdCards.IdCardNum as Docs
                                                    FROM Persons
                                                        INNER JOIN IdCards ON(IdCards.PersonID = Persons.PersonID)
                                                        INNER JOIN SourceReference as IDReference ON(IdCards.IdCardID = IDReference.RecordID)
                                                        INNER JOIN SourceReference ON Persons.PersonID = SourceReference.RecordID
                                                    WHERE IDReference.ReferenceTable = 7
                                                            AND Persons.PersonID = {0}
                                                            AND SourceReference.ReferenceTable = 1
                                                            AND SourceReference.Status = 1
                                                            AND IDReference.Status = 1 
                                                    UNION 
                                                    SELECT DISTINCT Passports.PassportNum as Docs
                                                    FROM Persons 
                                                        INNER JOIN SourceReference ON Persons.PersonID = SourceReference.RecordID 
                                                        INNER JOIN Passports ON(Passports.PersonID = Persons.PersonID) 
                                                        INNER JOIN SourceReference as passReference ON(Passports.PassportID = passReference.RecordID) 
                                                    WHERE passReference.ReferenceTable = 2 
                                                     AND passReference.Status = 1 
                                                     AND SourceReference.ReferenceTable = 1 
                                                     AND SourceReference.Status = 1
                                                    AND Persons.PersonID = {0}", personID);
                    docsList = Acra3DB.RawSqlQuery<string>(sql, p => new string(p["Docs"].ToString()));
                }
                catch (Exception ex)
                {
                    // _logger.Log.ErrorFormat("ACRAID Exception:{0}", ex.Message);
                }
                _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
                return docsList;
            }
        }

        //AB: TODO
        public bool IsPrevDataChanged(Models.PDataModel person)
        {
            //bool result = false;
            //using (Acra4DbContext context = new Acra4DbContext(_acra4DbOptions))
            //{
            //    var avvPersonID = context.AVVPersons.Where(p => p.PNum == person.Ssn).FirstOrDefault()?.ID;
            //    if (avvPersonID != null)
            //    {
            //        foreach (var item in person.PassportData.AvvDocuments.AvvDocument)
            //        {
            //            if(context.AVVDocuments.Where(d=>d.AVVPersonID == avvPersonID && d.DocumentNumber == item.DocumentIdentifier.DocumentNumber).FirstOrDefault() != null)
            //            {
            //               
            //                /*Check All Fields*/

            //            }
            //        }
            //    }
            //}            
            return false;
        }

        //AB: TODO
        public int? ComputeACRAID(string SSN, int PersonID)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
            {
                BPR_Persons person = System.Linq.Queryable.Where(context.BPR_Persons, p => p.PNum == SSN).First();
                int ACRAID = 0;
                if (person.ACRAID == null)
                {
                    ACRAID = GenerateACRAID(SSN);
                    person.ACRAID = ACRAID;

                    context.BPR_Persons.Update(person);
                }

                ACRAPersonMapper personMapper = System.Linq.Queryable.Where(context.ACRAPersonMappers, m => m.PersonID == PersonID).FirstOrDefault();

                if (personMapper != null)
                {
                    context.ACRAPersonMapperActivities.Add(new ACRAPersonMapperActivity { ACRAID = personMapper.ACRAID, PersonID = PersonID, isRemoved = true, ActionDate = DateTime.Now });
                    personMapper.ACRAID = Convert.ToInt32(ACRAID);
                    context.ACRAPersonMappers.Update(personMapper);
                }
                else
                {
                    context.ACRAPersonMapperActivities.Add(new ACRAPersonMapperActivity { ACRAID = ACRAID, PersonID = PersonID, isRemoved = false, ActionDate = DateTime.Now });
                    context.ACRAPersonMappers.Add(new ACRAPersonMapper { ACRAID = Convert.ToInt32(ACRAID), PersonID = PersonID });
                }

                context.SaveChanges();
                _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
                return ACRAID;
            }
        }

       
        private int GenerateACRAID(string SSN)
        {
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
            {
                _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");

                var acraIdentity = System.Linq.Queryable.Where(context.ACRAIdentities, p => p.IsLegal == false && p.ACRAGroup == SSN).FirstOrDefault();
                if (acraIdentity != null)
                    return acraIdentity.ACRAID;
                else
                {
                    ACRAIdentity identity = new ACRAIdentity() { ACRAGroup = SSN, IsLegal = false};
                    context.Add(identity);
                    context.SaveChanges();
                    return identity.ACRAID;
                }                    
            }
        }

        private void Get3rdSourceInfo(string SSN)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            Models.BySSN personData = new Models.BySSN { ssn = SSN };
            var url = "http://localhost:9070/Ekeng/GetPersonInfoBySSN";
           
            var client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new  StringContent(JsonConvert.SerializeObject(personData), Encoding.UTF8,"application/json") };
            HttpResponseMessage response = client.SendAsync(request).Result;

            Log3rdSourceRequests(request, response);
            if (response.IsSuccessStatusCode)
            {
                responseModel = JsonConvert.DeserializeObject<Models.PDataModel>(response.Content.ReadAsStringAsync().Result);              
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

        //AB TODO:
        private bool ACRAIDCleaner(string SSN)
        {
            return true;
        }

        private Models.PNumModel Get3rdSourceSSN(string Document)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            Models.ByDocument personData = new Models.ByDocument { documentNumber = Document };
            var url = "http://localhost:9070/Ekeng/GetPersonInfoByDocument";
            
            var client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(JsonConvert.SerializeObject(personData), Encoding.UTF8, "application/json") };
            HttpResponseMessage response = client.SendAsync(request).Result;
            Log3rdSourceRequests(request, response);
            Models.PNumModel pNum = new Models.PNumModel();

            if (response.IsSuccessStatusCode)
            {
                pNum = JsonConvert.DeserializeObject<Models.PNumModel>(response.Content.ReadAsStringAsync().Result);              
            }
            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
            return pNum;
        }

        //private void AddOrUpdateAVVData()
        //{
        //    using (Acra4DbContext context = new Acra4DbContext(_acra4DbOptions))
        //    {
        //        AcraData.Models.Acra4.AVVPerson avvPerson = new AcraData.Models.Acra4.AVVPerson();
        //        context.AVVPersons
        //    }   
        //}
    }
}
