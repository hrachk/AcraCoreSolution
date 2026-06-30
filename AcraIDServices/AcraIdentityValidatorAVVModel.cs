using AcraData.Data;
using AcraData.Models.Acra4;
using AcraIDServices.Mappers;
using AcraUtils;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using AcraData.Models.Acra3;
using System.Reflection;

namespace AcraIDServices
{
    public class AcraIdentityValidatorAVVModel
    {
        AVVMapper _avvMapper;
        private Logger _logger;
        DbContextOptions<Acra3DbContext> _acra3DbOptions;
        DbContextOptions<Acra4DbContext> _acra4DbOptions;
        DbContextOptions<AcraJournalDbContext> _acraJournalOptions;

        private Models.AVV.AvvResponse responseModel = new Models.AVV.AvvResponse();       

        public AcraIdentityValidatorAVVModel(DbContextOptions<Acra3DbContext> acra3dbOptions, DbContextOptions<Acra4DbContext> acra4dbOptions, Logger logger, DbContextOptions<AcraJournalDbContext> acraJournalOptions)
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
                        if ((responseModel.Result == null) || (responseModel.Result.Count != 1))
                            return null;
                        if (IsPrevDataChanged(responseModel.Result.FirstOrDefault()))
                            ACRAIDCleaner(SSN);
                        if (_avvMapper.ImportPerson(responseModel.Result.FirstOrDefault()) == null)
                            return null;
                        return AcraIdentityValidator(SSN, PersonID);
                    }
                }
            }
            else
            {
                Get3rdSourceInfo(SSN);
                if ((responseModel.Result == null) || (responseModel.Result.Count != 1))
                    return null;
                if (IsPrevDataChanged(responseModel.Result.FirstOrDefault()))
                    ACRAIDCleaner(SSN);                
                if (_avvMapper.ImportPerson(responseModel.Result.FirstOrDefault()) == null)
                    return null;
                return AcraIdentityValidator(SSN, PersonID);
            }
        }

        public int? AcraIdentityService(int PersonID)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            string SSN = GetSSNFromACRA(PersonID);            
            if (!string.IsNullOrEmpty(SSN))
                return AcraIdentityValidator(SSN, PersonID);
            else
            {
                SSN = GetSSNFrom3rdSource(PersonID);
                if (!string.IsNullOrEmpty(SSN))
                    return AcraIdentityValidator(SSN, PersonID);
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
                Models.AVV.AvvResponse pNum = Get3rdSourceSSN(personDocs[i]);
                if (pNum != null)
                {
                    if (i > 0)
                        isSSNOK = SSN.Equals(pNum.Result.FirstOrDefault().PNum.Trim());
                    else
                        SSN = pNum.Result.FirstOrDefault().PNum.Trim();
                }
                i++;
            }
            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
            return (isSSNOK) ? SSN : string.Empty;
        }

        public string GetSSNFrom3rdSource(string SSN)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");

            using var context = new Acra4DbContext(_acra4DbOptions);

            return context.BPR_Persons
                .FirstOrDefault(p => p.PNum == SSN || p.CertificateNum == SSN)
                ?.PNum;
        }

 

        public string GetSSNFromACRA(int PersonID)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");

            using var context = new Acra3DbContext(_acra3DbOptions);

            return context.Persons
                .FirstOrDefault(p => p.PersonId == PersonID)
                ?.SocialCard ?? string.Empty;
        }


        public bool SSNExistanceIn3rdSource(string SSN)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");

            using var context = new Acra4DbContext(_acra4DbOptions);

            return context.BPR_Persons
                .Any(p => p.PNum == SSN || p.CertificateNum == SSN);
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

            using var context = new Acra4DbContext(_acra4DbOptions);

            var person = context.BPR_Persons
                .FirstOrDefault(p => p.PNum == SSN);

            return person?.AVVGetDate?.Date == DateTime.Today;
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
        public bool IsPrevDataChanged(Models.AVV.BPR_Persons person)
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
            return true;
        }

        //AB: TODO
        public int? ComputeACRAID(string SSN, int PersonID)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");

            using var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions);

            var person = context.BPR_Persons
                .FirstOrDefault(p => p.PNum == SSN);

            if (person == null)
            {
                _logger.Log.Warn($"Person not found for SSN: {SSN}");
                return null;
            }

            int acraId;

            if (person.ACRAID.HasValue)
            {
                acraId = person.ACRAID.Value;
            }
            else
            {
                acraId = GenerateACRAID(SSN);
                person.ACRAID = acraId;
            }

            var personMapper = context.ACRAPersonMappers
                .FirstOrDefault(m => m.PersonID == PersonID);

            if (personMapper != null)
            {
                // activity: old mapping removed
                context.ACRAPersonMapperActivities.Add(new ACRAPersonMapperActivity
                {
                    ACRAID = personMapper.ACRAID,
                    PersonID = PersonID,
                    isRemoved = true,
                    ActionDate = DateTime.Now
                });

                personMapper.ACRAID = acraId;
            }
            else
            {
                // activity: new mapping created
                context.ACRAPersonMapperActivities.Add(new ACRAPersonMapperActivity
                {
                    ACRAID = acraId,
                    PersonID = PersonID,
                    isRemoved = false,
                    ActionDate = DateTime.Now
                });

                context.ACRAPersonMappers.Add(new ACRAPersonMapper
                {
                    ACRAID = acraId,
                    PersonID = PersonID,
                    Status = 1
                });
            }

            context.SaveChanges();

            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");

            return acraId;
        }


        //AB: TODO
        private int GenerateACRAID(string SSN)
        {
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
            {
                _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");

                var acraIdentity = context.ACRAIdentities.FirstOrDefault(p => p.IsLegal == false && p.ACRAGroup == SSN);
                if (acraIdentity != null)
                    return acraIdentity.ACRAID;
                else
                {
                    ACRAIdentity identity = new ACRAIdentity() { ACRAGroup = SSN, IsLegal = false };
                    context.Add(identity);
                    context.SaveChanges();
                    return identity.ACRAID;
                }
            }
        }

        private void Get3rdSourceInfo(string SSN)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            Models.AVV.BySSN personData = new Models.AVV.BySSN { psn = SSN, Addresses = Models.AVV.Addresses.CURRENT };
            var url = "http://localhost:9070/AVV/GetPersonInfoBySSN";

            var client = new HttpClient();
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

        //AB TODO:
        private bool ACRAIDCleaner(string SSN)
        {
            return true;
        }

        private Models.AVV.AvvResponse Get3rdSourceSSN(string Document)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            Models.AVV.ByDocument personData = new Models.AVV.ByDocument { docnum = Document,Addresses = Models.AVV.Addresses.CURRENT };
            var url = "http://localhost:9070/AVV/GetPersonInfoByDocument";

            var client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(JsonConvert.SerializeObject(personData), Encoding.UTF8, "application/json") };
            HttpResponseMessage response = client.SendAsync(request).Result;
            Log3rdSourceRequests(request, response);

            Models.AVV.AvvResponse result = new Models.AVV.AvvResponse();

            if (response.IsSuccessStatusCode)
            {
                result = JsonConvert.DeserializeObject<Models.AVV.AvvResponse>(response.Content.ReadAsStringAsync().Result);
            }
            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
            return result;
        }
    
    }
}
