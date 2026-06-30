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
using AcraIDServices;
using AcraUtils.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace AcraIDServices
{
    public class CollectAVVInfoService : AcraService
    {
        private CollectAVVInfoModel Model;
        private DbContextOptions<Acra3DbContext> _dbContextOptions;
        private DbContextOptions<Acra4DbContext> _acra4DbOptions;
        private DbContextOptions<AcraJournalDbContext> _acraJournalOptions;

        public CollectAVVInfoService(DbContextOptions<Acra3DbContext> acra3ContextOptions, DbContextOptions<AcraJournalDbContext> acraJournalOptions, DbContextOptions<Acra4DbContext> acra4ContextOptions, Logger logger) : base(logger)
        {
            _dbContextOptions = acra3ContextOptions;
            _acra4DbOptions = acra4ContextOptions;
            _acraJournalOptions = acraJournalOptions;
        }

        protected override void process()
        {
            try
            {
                _logger.Log.Info("AcraIdentityValidator Service has been started");               
                while (true)
                {
                    CheckCancel();
                    processItemsWithThread();                  
                    Thread.Sleep(1000);
                    _logger.Log.Info("Tick");
                }
            }
            catch (AggregateException aggregateEx)
            {
                _logger.Log.Info("Task Cancelled");
                aggregateEx.Handle(cancelEx => true);
            }
            catch (Exception ex)
            {
                _logger.Log.FatalFormat("AcraIdentityValidator Service process(): ExpMessage: {0} InnerExpMessage: {1}", ex.Message, ex.InnerException.Message);
            }

            base.process();
        }

        protected void processItemsWithThread()
        {
            using (var context = new Acra3DbContext(_dbContextOptions))
            {
                while (context.TriggerActivityTmps.Any(p => p.Status == 0 && p.ActivityType == 11))
                {
                    try
                    {
                        List<Task> tasksList = new List<Task>();
                        //TODO: Real 600
                        // int oneTimeCount = 50;
                        //int oneTimeCount = 300;
                        int oneTimeCount = 2500;
                        try
                        {
                            var activityList = System.Linq.Queryable
     .Where(context.TriggerActivityTmps, p => p.Status == 0 && p.ActivityType == 11)
     .OrderBy(t => t.Id)
     .Take(oneTimeCount)
     .ToList();


                            List<object> entityInfos = new List<object>();
                            GetEntityInfos(string.Join(", ", activityList.Select(p => p.ActivityId).ToList()), out entityInfos);

                            CollectAVVInfoModel.PersonsInfos = entityInfos.Cast<Models.PersonInfo>().ToList();

                            foreach (var item in activityList)
                            {
                                Model = new CollectAVVInfoModel(_dbContextOptions, _acra4DbOptions, _logger, _acraJournalOptions);
                                Task task = new Task(() => Model.Activity(item));
                                task.Start();
                                tasksList.Add(task);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Log.ErrorFormat("processItemsWithThread: ExpMessage: {0} InnerExpMessage: {1}", ex.Message, ex.InnerException.Message);
                        }

                        Task.WaitAll(tasksList.ToArray());
                        CheckCancel();
                    }
                    catch (Exception ex)
                    {
                        _logger.Log.ErrorFormat("processItemsWithThread: ExpMessage: {0} InnerExpMessage: {1}", ex.Message, ex.InnerException.Message);
                    }
                }
            }
        }

        private void AcraIdentityUpdate()
        {
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
            {
                _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");

                if (context.ACRAIdentities.FirstOrDefault() == null)
                {
                    //var IDNums = AcraIdentityValidatorBankIDModel_Person.BankIDs.Where(p => !string.IsNullOrEmpty(p.SocialCard)).Select(p => new { p.SocialCard }).Distinct();

                    var bankIDNums = AcraIdentityValidatorBankIDModel_Person.BankIDs.Select(p => new { p.BankID }).Distinct();

                    var maxIdentity = System.Linq.Queryable.Where(context.ACRAIdentities,   p => p.ACRAID < 550000000).OrderByDescending(p => p.ACRAID).FirstOrDefault();
                    int ACRAID = 0;
                    if (maxIdentity == null)
                        ACRAID = 1;
                    else
                        ACRAID = maxIdentity.ACRAID + 1;

                    List<ACRAIdentity> acraIdentities = new List<ACRAIdentity>();
                    foreach (var idNum in bankIDNums)
                    {
                        //var acraIdentity = context.ACRAIdentities.Where(p => p.IsLegal == false && p.ACRAGroup == idNum.BankID.ToString().Trim()).FirstOrDefault();
                       // if (acraIdentity == null)
                        {
                            ACRAIdentity identity = new ACRAIdentity() { ACRAID = ACRAID, ACRAGroup = idNum.BankID.ToString().Trim(), IsLegal = false };
                            acraIdentities.Add(identity);
                        }
                        ACRAID++;
                    }


                    //foreach (var idNum in IDNums)
                    //{
                    //    var acraIdentity = context.ACRAIdentities.Where(p => p.IsLegal == false && p.ACRAGroup == idNum.SocialCard.ToString().Trim()).FirstOrDefault();
                    //    if (acraIdentity == null)
                    //    {
                    //        ACRAIdentity identity = new ACRAIdentity() { ACRAID = ACRAID, ACRAGroup = idNum.SocialCard.ToString().Trim(), IsLegal = false };
                    //        acraIdentities.Add(identity);
                    //    }
                    //    ACRAID++;
                    //}
                    //ACRAID++;
                    //var _IDNums = AcraIdentityValidatorBankIDModel_Person.BankIDs.Where(p => !string.IsNullOrEmpty(p.HasNSocialCard)).Select(p => new { p.HasNSocialCard }).Distinct();

                    //foreach (var idNum in _IDNums)
                    //{
                    //    var acraIdentity = context.ACRAIdentities.Where(p => p.IsLegal == false && p.ACRAGroup == idNum.HasNSocialCard.ToString().Trim()).FirstOrDefault();
                    //    if (acraIdentity == null)
                    //    {
                    //        ACRAIdentity identity = new ACRAIdentity() { ACRAID = ACRAID, ACRAGroup = idNum.HasNSocialCard.ToString().Trim(), IsLegal = false };
                    //        acraIdentities.Add(identity);
                    //    }

                    //    ACRAID++;
                    //}

                    context.AddRange(acraIdentities);
                    context.SaveChanges();

                }
            }
        }        

        public void GetEntityInfos(string EntityIDs, out List<object> EntityInfos)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using (var IdentDB = new Acra3DbContext(_dbContextOptions))
            {
                string sql = $" SELECT DISTINCT PersonId, SocialCard, DATE_FORMAT(BirthDate,\"%Y-%m-%d\") as BirthDate, FirstName, LastName, DocumentNum FROM(" +
                         $"SELECT Persons.PersonId, Persons.SocialCard as SocialCard, Persons.BirthDate, Persons.FirstName, Persons.LastName, Passports.PassportNum as DocumentNum" +
                         $" FROM Persons " +
                         $" INNER JOIN SourceReference ON Persons.PersonId = SourceReference.RecordID AND SourceReference.ReferenceTable = 1 AND SourceReference. STATUS = 1" +
                         $" INNER JOIN Passports ON(Passports.PersonId = Persons.PersonId) " +
                         $" INNER JOIN SourceReference AS ValidPassport ON(ValidPassport.ReferenceTable = 2 AND ValidPassport.Status = 1 AND Passports.PassportID = ValidPassport.RecordID) " +
                         $" WHERE  Persons.PersonId IN({EntityIDs}) " +
                         $" UNION ALL" +
                         $"  SELECT Persons.PersonId, Persons.SocialCard as SocialCard, Persons.BirthDate, Persons.FirstName, Persons.LastName,IdCards.IdCardNum as DocumentNum" +
                         $" FROM Persons " +
                         $" INNER JOIN SourceReference ON Persons.PersonId = SourceReference.RecordID AND SourceReference.ReferenceTable = 1 AND SourceReference. STATUS = 1" +
                         $" INNER JOIN IdCards ON(IdCards.PersonId = Persons.PersonId) " +
                         $" INNER JOIN SourceReference AS SourceRef ON(SourceRef.ReferenceTable = 7 AND SourceRef.Status = 1 AND IdCards.IdCardID = SourceRef.RecordID) " +
                         $" WHERE  Persons.PersonId IN({EntityIDs})) as Info";

                _logger.Log.Info($"SQL:{sql}");
                EntityInfos = IdentDB.RawSqlQuery<object>(sql, p => new Models.PersonInfo
                {
                    PersonId = Convert.ToInt32(p["PersonId"]),
                    SocialCard = p["SocialCard"].ToString(),
                    //BirthDate = Convert.ToDateTime(p["BirthDate"].ToString()),
                    BirthDate = p["BirthDate"].ToString(), // (p["BirthDate"].ToString() != null && p["BirthDate"].ToString().Contains("00")) ? Convert.ToDateTime(p["BirthDate"].ToString()) : default(DateTime),
                    FirstName = p["FirstName"].ToString(),
                    LastName = p["LastName"].ToString(),
                    DocumentNum = p["DocumentNum"].ToString()
                });
            }
        }
    }
}
