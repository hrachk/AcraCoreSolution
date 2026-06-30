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
    public class AcraIdentityValidatorService : AcraService
    {
        private dynamic Model;
        private DbContextOptions<Acra3DbContext> _dbContextOptions;
        private DbContextOptions<Acra4DbContext> _acra4DbOptions;
        private DbContextOptions<AcraJournalDbContext> _acraJournalOptions;
        private ValidatorType _validatorType;

        public AcraIdentityValidatorService(ValidatorType validatorType,DbContextOptions<Acra3DbContext> acra3ContextOptions, DbContextOptions<AcraJournalDbContext> acraJournalOptions, DbContextOptions<Acra4DbContext> acra4ContextOptions, Logger logger) : base(logger)
        {
            _dbContextOptions = acra3ContextOptions;
            _acra4DbOptions = acra4ContextOptions;
            _validatorType = validatorType;
            _acraJournalOptions = acraJournalOptions;
            switch (validatorType)
            {
                case ValidatorType.BANKID:                    
                    Model = new AcraIdentityValidatorBankIDModel(acra3ContextOptions, acra4ContextOptions, _logger);
                    break;
                case ValidatorType.AVV:
                    Model = new AcraIdentityValidatorAVVModel(acra3ContextOptions, acra4ContextOptions, _logger, acraJournalOptions);
                    break;
                case ValidatorType.EKENG:
                    Model = new AcraIdentityValidatorEkengModel(acra3ContextOptions, acra4ContextOptions, _logger, acraJournalOptions);
                    break;
                default:
                    break;
            }            
        }

        protected override void process()
        {
            try
            {
                _logger.Log.Info("AcraIdentityValidator Service has been started");

                using (var context = new Acra3DbContext(_dbContextOptions))
                {
                    if (_validatorType == ValidatorType.BANKID)
                    {
                        var _activities = System.Linq.Queryable.Where(context.MonitoringPlusActivityTmps, p => p.Status == 2 && p.ActivityType == 9).ToList();
                        _activities.ForEach(p => { p.Status = 0; });
                        context.MonitoringPlusActivityTmps.UpdateRange(_activities);
                        context.SaveChanges();
                    }
                }
                while (true)
                {
                    CheckCancel();
                   processItemsWithThreadBankID();
                    //processItems();
                   // processItemsWithThread();
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

        protected void processItems()
        {
            using (var acra4DbContext = new Acra4DbContext(_acra4DbOptions))
            {
                using (var context = new Acra3DbContext(_dbContextOptions))
                {
                    try
                    {
                        switch (_validatorType)
                        {
                            case ValidatorType.BANKID:
                              
                                AcraIdentityValidatorBankIDModel_Person.BankIDs = context.BankIDs.ToList();
                                AcraIdentityValidatorBankIDModel_Legal.BankIDLegals = context.BankIDLegals.ToList();


                                //var activities =  context.MonitoringPlusActivityTmps.Where(p => p.ActivityType == 10 && p.Status == 100).ToList();
                                //activities.ForEach(p => { p.Status = 1; });
                                //context.MonitoringPlusActivityTmps.UpdateRange(activities);
                                //context.SaveChanges();

                                AcraIdentityUpdate();


                                AcraIdentityValidatorBankIDModel_Person.ACRAIDs = acra4DbContext.ACRAIdentities.ToList();

                                while (context.MonitoringPlusActivityTmps.Any(t => t.Status == 1 && t.ActivityType == 10))
                                {
                                  
                                    Model.Activity(10, System.Linq.Queryable.Where(context.MonitoringPlusActivityTmps, t => t.Status == 1 && t.ActivityType == 10).OrderBy(t => t.Id).Take(50).ToList());                                    
                                    CheckCancel();
                                }




                                if (context.MonitoringPlusActivityTmps.Any(t => t.Status == 0 && (t.ActivityType == 9 || t.ActivityType == 10)))
                                {
                                    foreach (var activityItem in System.Linq.Queryable.Where(context.MonitoringPlusActivityTmps, t => t.Status == 0 && (t.ActivityType == 9 || t.ActivityType == 10)).OrderBy(t => t.Id).Take(50).ToList())
                                    {
                                        Model.Activity(activityItem);

                                        CheckCancel();
                                    }
                                }
                                break;
                            case ValidatorType.AVV:
                                if (context.TriggerActivityTmps.Any(t => t.Status == 0 && t.ActivityType == 6))
                                {
                                    foreach (var activityItem in System.Linq.Queryable.Where(context.TriggerActivityTmps, t => t.Status == 0 && t.ActivityType == 6).OrderBy(t => t.Id).Take(50).ToList())
                                    {
                                        Model.Activity(activityItem);

                                        CheckCancel();
                                    }
                                }
                                break;
                            case ValidatorType.EKENG:
                                if (context.TriggerActivityTmps.Any(t => t.Status == 0 && t.ActivityType == 6))
                                {
                                    foreach (var activityItem in System.Linq.Queryable.Where(context.TriggerActivityTmps, t => t.Status == 0 && t.ActivityType == 6).OrderBy(t => t.Id).Take(50).ToList())
                                    {
                                        Model.Activity(activityItem);

                                        CheckCancel();
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Log.ErrorFormat("processItems: ExpMessage: {0} InnerExpMessage: {1}", ex.Message, ex.InnerException.Message);
                    }
                }
            }
        }

        protected void processItemsWithThreadBankID()
        {
            using (var context = new Acra3DbContext(_dbContextOptions))
            {
                using (var acra4DbContext = new Acra4DbContext(_acra4DbOptions))
                {
                    try
                    {
                        switch (_validatorType)
                        {
                            case ValidatorType.BANKID:

                               // AcraIdentityValidatorBankIDModel_Person.BankIDs = context.BankIDs.ToList();
                              //  AcraIdentityValidatorBankIDModel_Legal.BankIDLegals = context.BankIDLegals.ToList();


                                //var _activities = context.MonitoringPlusActivityTmps.Where(p => p.ActivityType == 10 && p.Status == 0).ToList();
                                //_activities.ForEach(p => { p.Status = 1; });
                                //context.MonitoringPlusActivityTmps.UpdateRange(_activities);
                                //context.SaveChanges();

                            //    AcraIdentityUpdate();


                                //AcraIdentityValidatorBankIDModel_Person.ACRAIDs = acra4DbContext.ACRAIdentities.ToList();
                                //long maxID = context.MonitoringPlusActivityTmps.Where(t => t.Status == 200 && t.ActivityType == 10).OrderByDescending(p=>p.Id).First().Id;
                                //while (context.MonitoringPlusActivityTmps.Any(t => t.Status == 1 && t.ActivityType == 10))
                                //{
                                //    List<Task> tasksList = new List<Task>();
                                //    //Model.Activity(10, context.MonitoringPlusActivityTmps.Where(t => t.Status == 1 && t.ActivityType == 10).OrderBy(t => t.Id).Take(50).ToList());
                                //    //CheckCancel();
                                //    int threadCount = 10;
                                //    int oneTimeCount = 50;                                    
                                //    for (int i = 0; i < threadCount; i++)
                                //    {
                                //        try
                                //        {
                                //            if (context.MonitoringPlusActivityTmps.Any(t => t.Status == 1 && t.ActivityType == 10 && t.Id > maxID))
                                //            {
                                //                var activities = context.MonitoringPlusActivityTmps.Where(t => t.Status == 1 && t.ActivityType == 10 && t.Id > maxID).OrderBy(t => t.Id).Take(oneTimeCount).ToList();
                                //                Task task = new Task(() => Model.Activity(10, activities));
                                //                task.Start();
                                //                tasksList.Add(task);
                                //                maxID = activities.OrderByDescending(p => p.Id).First().Id;
                                //            }
                                //        }
                                //        catch (Exception ex)
                                //        {
                                //            _logger.Log.ErrorFormat("processItemsWithThread: ExpMessage: {0} InnerExpMessage: {1}", ex.Message, ex.InnerException.Message);
                                //        }
                                //    }

                                //    Task.WaitAll(tasksList.ToArray());
                                //    CheckCancel();
                                //}


                                if (context.MonitoringPlusActivityTmps.Any(t => t.Status == 0 && (t.ActivityType == 9 || t.ActivityType == 10)))
                                {
                                    foreach (var activityItem in System.Linq.Queryable.Where(context.MonitoringPlusActivityTmps, t => t.Status == 0 && (t.ActivityType == 9 || t.ActivityType == 10)).OrderBy(t => t.Id).Take(100).ToList())
                                    {
                                        Model.Activity(activityItem);

                                        CheckCancel();
                                    }
                                }
                                break;                                
                            case ValidatorType.AVV:
                                if (context.TriggerActivityTmps.Any(t => t.Status == 0 && t.ActivityType == 6))
                                {
                                    foreach (var activityItem in System.Linq.Queryable.Where(context.TriggerActivityTmps, t => t.Status == 0 && t.ActivityType == 6).OrderBy(t => t.Id).Take(50).ToList())
                                    {
                                        Model.Activity(activityItem);

                                        CheckCancel();
                                    }
                                }
                                break;
                            case ValidatorType.EKENG:
                                if (context.TriggerActivityTmps.Any(t => t.Status == 0 && t.ActivityType == 6))
                                {
                                    foreach (var activityItem in System.Linq.Queryable.Where(context.TriggerActivityTmps, t => t.Status == 0 && t.ActivityType == 6).OrderBy(t => t.Id).Take(50).ToList())
                                    {
                                        Model.Activity(activityItem);

                                        CheckCancel();
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Log.ErrorFormat("processItemsWithThread: ExpMessage: {0} InnerExpMessage: {1}", ex.Message, ex.InnerException.Message);
                    }
                }
            }
        }

        protected void processItemsWithThread()
        {
            using (var context = new Acra3DbContext(_dbContextOptions))
            {
                using (var acra4DbContext = new Acra4DbContext(_acra4DbOptions))
                {
                    try
                    {
                        switch (_validatorType)
                        {
                            case ValidatorType.BANKID:

                                AcraIdentityValidatorBankIDModel_Person.BankIDs = context.BankIDs.ToList();
                                AcraIdentityValidatorBankIDModel_Legal.BankIDLegals = context.BankIDLegals.ToList();


                                // var _activities = context.MonitoringPlusActivityTmps.Where(p => p.ActivityType == 10 && p.Status == 0).ToList();
                                //_activities.ForEach(p => { p.Status = 1; });
                                //context.MonitoringPlusActivityTmps.UpdateRange(_activities);
                                //context.SaveChanges();

                                AcraIdentityUpdate();


                                AcraIdentityValidatorBankIDModel_Person.ACRAIDs = acra4DbContext.ACRAIdentities.ToList();

                                long maxID = 0;
                                while (context.MonitoringPlusActivityTmps.Any(t => t.Status == 1 && t.ActivityType == 10))
                                {
                                    List<Task> tasksList = new List<Task>();
                                    //Model.Activity(10, context.MonitoringPlusActivityTmps.Where(t => t.Status == 1 && t.ActivityType == 10).OrderBy(t => t.Id).Take(50).ToList());
                                    //CheckCancel();
                                    int threadCount = 10;
                                    int oneTimeCount = 50;
                                    for (int i = 0; i < threadCount; i++)
                                    {
                                        try
                                        {
                                            if (context.MonitoringPlusActivityTmps.Any(t => t.Status == 1 && t.ActivityType == 10 && t.Id > maxID))
                                            {
                                                var activities = System.Linq.Queryable.Where(context.MonitoringPlusActivityTmps, t => t.Status == 1 && t.ActivityType == 10 && t.Id > maxID).OrderBy(t => t.Id).Take(oneTimeCount).ToList();
                                                Task task = new Task(() => Model.Activity(10, activities));
                                                task.Start();
                                                tasksList.Add(task);
                                                maxID = activities.OrderByDescending(p => p.Id).First().Id;
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Log.ErrorFormat("processItemsWithThread: ExpMessage: {0} InnerExpMessage: {1}", ex.Message, ex.InnerException.Message);
                                        }
                                    }

                                    Task.WaitAll(tasksList.ToArray());
                                    CheckCancel();
                                }

                                if (context.MonitoringPlusActivityTmps.Any(t => t.Status == 1 && t.ActivityType == 10))
                                {
                                    List<Task> tasksList = new List<Task>();

                                    foreach (var activityItem in System.Linq.Queryable.Where(context.MonitoringPlusActivityTmps, t => t.Status == 0 && (t.ActivityType == 9 || t.ActivityType == 10)).OrderBy(t => t.Id).Take(100).ToList())
                                    {
                                        try
                                        {
                                            Task task = new Task(() => Model.Activity(activityItem));
                                            task.Start();
                                            tasksList.Add(task);
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Log.ErrorFormat("processItemsWithThread: ExpMessage: {0} InnerExpMessage: {1}", ex.Message, ex.InnerException.Message);
                                        }
                                    }
                                    Task.WaitAll(tasksList.ToArray());
                                    CheckCancel();
                                }

                                if (context.MonitoringPlusActivityTmps.Any(t => t.Status == 0 && (t.ActivityType == 9 || t.ActivityType == 10)))
                                {
                                    List<Task> tasksList = new List<Task>();

                                    foreach (var activityItem in System.Linq.Queryable.Where(context.MonitoringPlusActivityTmps, t => t.Status == 0 && (t.ActivityType == 9 || t.ActivityType == 10)).OrderBy(t => t.Id).Take(10).ToList())
                                    {
                                        try
                                        {
                                            Task task = new Task(() => Model.Activity(activityItem));
                                            task.Start();
                                            tasksList.Add(task);
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Log.ErrorFormat("processItemsWithThread: ExpMessage: {0} InnerExpMessage: {1}", ex.Message, ex.InnerException.Message);
                                        }
                                    }
                                    Task.WaitAll(tasksList.ToArray());
                                    CheckCancel();
                                }
                                break;
                            case ValidatorType.AVV:
                                if (context.TriggerActivityTmps.Any(t => t.Status == 0 && t.ActivityType == 6))
                                {
                                    foreach (var activityItem in System.Linq.Queryable.Where(context.TriggerActivityTmps, t => t.Status == 0 && t.ActivityType == 6).OrderBy(t => t.Id).Take(50).ToList())
                                    {
                                        Model.Activity(activityItem);

                                        CheckCancel();
                                    }
                                }
                                break;
                            case ValidatorType.EKENG:
                                if (context.TriggerActivityTmps.Any(t => t.Status == 0 && t.ActivityType == 6))
                                {
                                    foreach (var activityItem in System.Linq.Queryable.Where(context.TriggerActivityTmps, t => t.Status == 0 && t.ActivityType == 6).OrderBy(t => t.Id).Take(50).ToList())
                                    {
                                        Model.Activity(activityItem);

                                        CheckCancel();
                                    }
                                }
                                break;
                            default:
                                break;
                        }
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

                    var maxIdentity = System.Linq.Queryable.Where(context.ACRAIdentities, p => p.ACRAID < 550000000).OrderByDescending(p => p.ACRAID).FirstOrDefault();
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

        public enum ValidatorType
        {
            BANKID,
            AVV,
            EKENG
        }
    }
}
