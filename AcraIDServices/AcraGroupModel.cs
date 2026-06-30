using AcraData.Data;
using AcraData.Models.Acra3;
using AcraUtils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AcraIDServices
{
    public class AcraGroupModel
    {
        DbContextOptions<Acra3DbContext> Acra3DBContextOptions;        
        Logger _logger;
        
        public AcraGroupModel(DbContextOptions<Acra3DbContext> acra3DbContext,  Logger logger)
        {
            Acra3DBContextOptions = acra3DbContext;            
            _logger = logger;
        }

        private bool RefreshPersonsList()
        {
            return true;
            //bool result = true;
            //_logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            //using (Acra3DbContext Acra3DB = new Acra3DbContext(Acra3DBContextOptions))
            //{
            //    using (var tx = Acra3DB.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var acraPersons = Acra3DB.Predicats.Where(p => !Acra3DB.ACRAGroups.Any(ag => ag.PersonID == p.PersonId) && p.IsDeleted == false)
            //                                     .Select(p => new ACRAGroup()
            //                                     {
            //                                         PersonID = p.PersonId,
            //                                         ACRAID = -1,
            //                                         StageID = 0,
            //                                         IncomingDate = DateTime.Now,
            //                                         Status = 1
            //                                     }).Distinct();

            //            Acra3DB.ACRAGroups.AddRange(acraPersons);
            //            tx.Commit();
            //            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
            //            return result;
            //        }
            //        catch (Exception ex)
            //        {
            //            tx.Rollback();                     
            //            _logger.Log.ErrorFormat("ACRAID RefreshPersonsList failed Error:{0}",  ex.Message);
            //            result = false;
            //            throw (new Exception("ACRAID RefreshPersonsList", ex));                        
            //        }
            //        finally
            //        {
            //            Acra3DB.SaveChanges();                       
            //        }
            //    }
            //}            
        }

        private void ComputeAcraGroupFirstStage()
        {
            ;
            //using (Acra3DbContext Acra3DB = new Acra3DbContext(Acra3DBContextOptions))
            //{
            //    using (var tx = Acra3DB.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var acraPersons = Acra3DB.ACRAGroups.Where(ag => ag.StageID == 0).OrderBy(ag => ag.PersonID);




            //            List<Predicat> items = Acra3DB.Predicats.Where(p => p.IsDeleted == false && !string.IsNullOrEmpty(p.AcraGroup)).OrderBy(p => p.ID).Take(1000).ToList();
            //            items.AsParallel().ForAll(item =>
            //             {




            //                 if (string.IsNullOrEmpty(item.AcraGroup))
            //                     item.AcraGroup = "-1";

            //             }
            //            );
            //            Acra3DB.SaveChanges();
            //            _logger.Log.Info(string.Format("ACRAID Set Criterias գործ. հաջողությամբ իրականացված է"));
            //        }
            //        catch (Exception ex)
            //        { }
            //    }
            //}
        }

        private void ComputeAcraGroupSecondStage()
        {
            //using (Acra3DbContext Acra3DB = new Acra3DbContext(Acra3DBContextOptions))
            //{
            //    List<Predicat> items = Acra3DB.Predicats.Where(p => p.IsDeleted == false && string.IsNullOrEmpty(p.AcraGroup)).OrderBy(p => p.ID).Take(1000).ToList();
            //    items.AsParallel().ForAll(item =>
            //    {

            //        //if (item.Criteria1 == null)
            //        //{
            //        //    AcraData.Models.CB.Register register = Criteria1(item);
            //        //    item.Criteria1 = register?.Id ?? null;
            //        //    item.SC1 = register?.SocCard ?? null;
            //        //}

            //        //if (item.Criteria2 == null)
            //        //{
            //        //    AcraData.Models.CB.Register register = Criteria2(item);
            //        //    item.Criteria2 = register?.Id ?? null;
            //        //    item.SC2 = register?.SocCard ?? null;
            //        //}

            //        //if (item.Criteria3 == null)
            //        //{
            //        //    AcraData.Models.CB.Register register = Criteria3(item);
            //        //    item.Criteria3 = register?.Id ?? null;
            //        //    item.SC3 = register?.SocCard ?? null;
            //        //}

            //        //if (item.Criteria4 == null)
            //        //{
            //        //    AcraData.Models.CB.Register register = Criteria4(item);
            //        //    item.Criteria4 = register?.Id ?? null;
            //        //    item.SC4 = register?.SocCard ?? null;
            //        //}

            //        //if (item.Criteria5 == null)
            //        //{
            //        //    AcraData.Models.CB.Register register = Criteria5(item);
            //        //    item.Criteria5 = register?.Id ?? null;
            //        //    item.SC5 = register?.SocCard ?? null;
            //        //}

            //        //if (item.Criteria6 == null)
            //        //{
            //        //    AcraData.Models.CB.Register register = Criteria6(item);
            //        //    item.Criteria6 = register?.Id ?? null;
            //        //    item.SC6 = register?.SocCard ?? null;
            //        //}

            //        //if (item.Criteria7 == null)
            //        //{
            //        //    AcraData.Models.CB.Register register = Criteria7(item);
            //        //    item.Criteria7 = register?.Id ?? null;
            //        //    item.SC7 = register?.SocCard ?? null;
            //        //}

            //        //if (item.Criteria8 == null)
            //        //{
            //        //    AcraData.Models.CB.Register register = Criteria8(item);
            //        //    item.Criteria8 = register?.Id ?? null;
            //        //    item.SC8 = register?.SocCard ?? null;
            //        //}

            //        //if (item.Criteria9 == null)
            //        //{
            //        //    AcraData.Models.CB.Register register = Criteria9(item);
            //        //    item.Criteria9 = register?.Id ?? null;
            //        //    item.SC9 = register?.SocCard ?? null;
            //        //}

            //        //if (item.Criteria10 == null)
            //        //{
            //        //    AcraData.Models.CB.Register register = Criteria10(item);
            //        //    item.Criteria10 = register?.Id ?? null;
            //        //    item.SC10 = register?.SocCard ?? null;
            //        //}

            //        //if (item.Criteria11 == null)
            //        //{
            //        //    AcraData.Models.CB.Register register = Criteria11(item);
            //        //    item.Criteria11 = register?.Id ?? null;
            //        //    item.SC11 = register?.SocCard ?? null;
            //        //}

                   
            //        if (string.IsNullOrEmpty(item.AcraGroup))
            //            item.AcraGroup = "-1";

            //    }
            //    );
            //    Acra3DB.SaveChanges();
            //    _logger.Log.Info(string.Format("ACRAID Set Criterias գործ. հաջողությամբ իրականացված է"));
            //}
        }

    
        //private int GetFirstNameId(string firstName)
        //{
        //    using (Acra3DbContext Acra3DB = new Acra3DbContext(Acra3DBContextOptions))
        //    {                
        //        return  Acra3DB.DicFirstNames.Where(f => f.FirstName == AcraUtils.Encodings.GetCP1252string(firstName)).OrderBy(df => df.FirstNameId).FirstOrDefault().FirstNameId;                
        //    }
        //}

        //private int GetLastNameId(string lastName)
        //{
        //    using (Acra3DbContext Acra3DB = new Acra3DbContext(Acra3DBContextOptions))
        //    {
        //        return Acra3DB.DicLastNames.Where(l => l.LastName == AcraUtils.Encodings.GetCP1252string(lastName)).OrderBy(lf => lf.LastNameId).FirstOrDefault().LastNameId;
        //    }
        //}

        public bool RefreshPersons()
        {

        // //   SetAllDeletedState();
        //    //List<AcraData.Models.Acra3.Predicat> PersonsList = GetPersonsList();

        ////    if (ActualPersons())            
        //        return AddPersons();            
        //    else
                return false;
        }

        public bool AcraGroup()
        {
            try
            {
                //FillPersons();
                //ComputeAcraGroup();
                return true;
            }
            catch { return false; }
        }
    }
}
