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
using AcraIDServices.Models;

namespace AcraIDServices
{
    public class AcraIdentityValidatorBankIDModel
    {
        AVVMapper _avvMapper;
        private Logger _logger;
        DbContextOptions<Acra3DbContext> _acra3DbOptions;
        DbContextOptions<Acra4DbContext> _acra4DbOptions;

        private Models.AVV.AvvResponse responseModel = new Models.AVV.AvvResponse();

        public AcraIdentityValidatorBankIDModel(DbContextOptions<Acra3DbContext> acra3dbOptions, DbContextOptions<Acra4DbContext> acra4dbOptions, Logger logger)
        {
            _acra3DbOptions = acra3dbOptions;
            _acra4DbOptions = acra4dbOptions;
            _logger = logger;
            _avvMapper = new AVVMapper(_logger, acra4dbOptions);
        }
       
        public void Activity(dynamic activityItem)
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
                            case 9:
                                AcraIdentityValidatorBankIDModel_Legal legal = new AcraIdentityValidatorBankIDModel_Legal(_acra3DbOptions, _acra4DbOptions, _logger);
                                ACRAID = legal.AcraIdentityService((int)activityItem.ActivityId);
                                break;
                            case 10:                                
                                AcraIdentityValidatorBankIDModel_Person person = new AcraIdentityValidatorBankIDModel_Person(_acra3DbOptions, _acra4DbOptions, _logger);
                                ACRAID = person.AcraIdentityService((int)activityItem.ActivityId);
                                break;
                            default:
                                break;
                        }
                        /* DELETE
                        //DB.LoanActivityTmps.Attach(activityItem);
                        //DB.LoanActivityTmps.Remove(activityItem);
                        */
                        activityItem.Status = (ACRAID == null) ? 100 : 200;
                        DB.Entry(activityItem).State = EntityState.Detached;
                        DB.MonitoringPlusActivityTmps.Update(activityItem);
                        DB.SaveChanges();
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        activityItem.Status = 2;
                        DB.Entry(activityItem).State = EntityState.Detached;
                        DB.MonitoringPlusActivityTmps.Update(activityItem);
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


        public void Activity(int activityType,dynamic activityItems)
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
                        switch (activityType)
                        {
                            //case 9:
                            //    AcraIdentityValidatorBankIDModel_Legal legal = new AcraIdentityValidatorBankIDModel_Legal(_acra3DbOptions, _acra4DbOptions, _logger);
                            //    ACRAID = legal.AcraIdentityService((int)activityItem.ActivityId);
                            //    break;
                            case 10:
                                AcraIdentityValidatorBankIDModel_Person person = new AcraIdentityValidatorBankIDModel_Person(_acra3DbOptions, _acra4DbOptions, _logger);
                                if (!person.SetACRAIDusingBankID(activityItems))
                                {
                                    ((List<MonitoringPlusActivityTemp>)activityItems).ForEach(p => { p.Status = 100; });                                   
                                    DB.MonitoringPlusActivityTmps.UpdateRange(activityItems);
                                }
                                break;
                            default:
                                break;
                        }
                        /* DELETE
                        //DB.LoanActivityTmps.Attach(activityItem);
                        //DB.LoanActivityTmps.Remove(activityItem);
                        */
                        //activityItem.Status = (ACRAID == null) ? 100 : 200;
                        //DB.Entry(activityItem).State = EntityState.Detached;
                        //DB.MonitoringPlusActivityTmps.Update(activityItem);
                        DB.SaveChanges();
                        tx.Commit();
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        ((List<MonitoringPlusActivityTemp>)activityItems).ForEach(p => { p.Status = 2; });
                        DB.MonitoringPlusActivityTmps.UpdateRange(activityItems);
                        _logger.Log.ErrorFormat("ACRAID Validataor Activity failed Error:{0}", ex.Message);
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

    }
}
