using AcraData.Data;
using AcraData.Models.Acra3;
using AcraData.Models.Acra4;
using AcraIDServices.Mappers;
using AcraIDServices.Models;
using AcraUtils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace AcraIDServices
{
    public class AcraIdentityValidatorBankIDModel_Legal : Person_AcraIdentityValidatorModel
    {
        AVVMapper _avvMapper;
        private Logger _logger;
        DbContextOptions<Acra3DbContext> _acra3DbOptions;
        DbContextOptions<Acra4DbContext> _acra4DbOptions;

        private Models.AVV.AvvResponse responseModel = new Models.AVV.AvvResponse();
        public static List<BankIDLegal> BankIDLegals;

        public AcraIdentityValidatorBankIDModel_Legal(DbContextOptions<Acra3DbContext> acra3dbOptions, DbContextOptions<Acra4DbContext> acra4dbOptions, Logger logger) : base(acra3dbOptions, acra4dbOptions, logger)
        {
            _acra3DbOptions = acra3dbOptions;
            _acra4DbOptions = acra4dbOptions;
            _logger = logger;
            _avvMapper = new AVVMapper(_logger, acra4dbOptions);
        }


        public override int? AcraIdentityService(int EntityID)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            return AcraIdentityValidator(EntityID);
        }

        public override int? AcraIdentityValidator(int EntityID)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            OrgInfo orgInfo = GetOrgInfoUTF8(EntityID);
            AcraData.Models.Acra3.BankIDLegal bankIDs = new BankIDLegal();

            using (Acra3DbContext context = new Acra3DbContext(_acra3DbOptions))
            {
                //if (!string.IsNullOrEmpty(orgInfo.HVHH))
                //{
                //    // 1. Check ANTP and HVHH
                //    bankIDs = BankIDLegals.Where(p => orgInfo.HVHH == p.ANTP).FirstOrDefault();

                //    if (bankIDs != null)
                //        return ComputeACRAID(orgInfo.HVHH, EntityID, bankIDs.BankID);
                //}
                return
                    ComputeACRAID(orgInfo.HVHH, EntityID, string.Empty);
            }
        }


        public override bool CheckAllDocsExistance(string IDNum, int EntityID)
        {
            throw new NotImplementedException();
        }

        public override int GenerateACRAID(string IDNum)
        {
            using var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions);

            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");

            // Ищем существующую запись
            var acraIdentity = context.ACRAIdentities
      .FirstOrDefault(p => p.IsLegal == true && p.ACRAGroup == IDNum);

            if (acraIdentity != null)
                return acraIdentity.ACRAID;

            // Находим максимальный ACRAID в диапазоне
            var maxIdentity = System.Linq.Queryable
       .Where(context.ACRAIdentities, p => p.ACRAID >= 990000000 && p.ACRAID < 999999999)
       .OrderByDescending(p => p.ACRAID)
       .FirstOrDefault();


            int newACRAID = (maxIdentity?.ACRAID ?? 990000000) + 1;

            var identity = new ACRAIdentity
            {
                ACRAID = newACRAID,
                ACRAGroup = IDNum,
                IsLegal = true
            };

            context.ACRAIdentities.Add(identity);
            context.SaveChanges();

            return identity.ACRAID;
        }


        public override dynamic Get3rdSourceIDNum(string Document)
        {
            throw new NotImplementedException();
        }

        public override void Get3rdSourceInfo(string IDNum, string url)
        {
            throw new NotImplementedException();
        }

        public override string GetIDNumFrom3rdSource(int EntityID)
        {
            throw new NotImplementedException();
        }

        public override string GetIDNumFrom3rdSource(string IDNum)
        {
            throw new NotImplementedException();
        }

        public override string GetIDNumFromACRA(int EntityID)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using (Acra3DbContext context = new Acra3DbContext(_acra3DbOptions))
            {
                return System.Linq.Queryable.Where(context.Persons, p => p.PersonId == EntityID).FirstOrDefault()?.SocialCard ?? string.Empty;
            }
        }

        public override List<string> GetPersonsDocs(int EntityID)
        {
            throw new NotImplementedException();
        }

        public override bool IDNumExistanceIn3rdSource(string IDNum)
        {
            throw new NotImplementedException();
        }

        public override bool Is3rdSourceUpToDate(string IDNum)
        {
            throw new NotImplementedException();
        }

        public override bool IsPrevDataChanged(dynamic person)
        {
            throw new NotImplementedException();
        }

        public override void Log3rdSourceRequests(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage)
        {
            throw new NotImplementedException();
        }

        public OrgInfo GetOrgInfoUTF8(int OrgId)
        {
            using (var IdentDB = new Acra3DbContext(_acra3DbOptions))
            {
                string sql = $"SELECT Organizations.OrganizationID " +
                        $" ,convert(cast(convert(Organizations.HVHH using  latin1) as binary) using utf8) as HVHH " +
                $" FROM Organizations " +
                $" INNER JOIN SourceReference ON(Organizations.OrganizationID = SourceReference.RecordID) " +
                $" LEFT JOIN OrganizationNames ON(SourceReference.RecordID = OrganizationNames.OrganizationID) " +
                $" LEFT JOIN SourceReference AS OrgNameRef ON( " +
                    $" OrgNameRef.STATUS = 1 " +
                    $" AND OrgNameRef.ReferenceTable = 6 " +
                    $" AND OrganizationNames.OrganizationNameID = OrgNameRef.RecordID )" +
                $" WHERE SourceReference.ReferenceTable = 5 " +
                $" AND SourceReference. STATUS = 1 " +
                $" AND Organizations.OrganizationID = {OrgId} " +
                $" GROUP BY Organizations.OrganizationID ";

                return IdentDB.RawSqlQuery<OrgInfo>(sql, p => new OrgInfo
                {
                    OrgId = Convert.ToInt32(p["OrganizationID"]),
                    HVHH = p["HVHH"].ToString()
                }).FirstOrDefault();
            }
        }

        /*
        public int ComputeScorePersonID(int PersonID, int? ACRAID)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
            {                
                if (ACRAID == null)
                {
                    var scorePersons = context.ACRAPersonMappers.Where(p => p.ACRAID >= 990000000 && p.ACRAID < 999999999).OrderByDescending(p => new { p.ACRAID }).First();
                    ACRAID = scorePersons.ACRAID + 1;
                }

                var _legalMapper = context.ACRALegalMappers.Where(m => m.OrganizationID == OrgID).Select(p => new { p.ACRAID, p.OrganizationID }).FirstOrDefault();

                if (_legalMapper != null)
                {
                    context.ACRALegalMapperActivities.Add(new ACRALegalMapperActivity { ACRAID = legalMapper.ACRAID, OrganizationID = OrgID, isRemoved = true, ActionDate = DateTime.Now });
                    context.ACRALegalMappers.Where(m => m.OrganizationID == OrgID).ForEachAsync(p => { p.ACRAID = ACRAID; p.BANKID = BankID; p.Status = 1; p.StageID = (string.IsNullOrEmpty(BankID)) ? (sbyte)2 : (sbyte)1; p.IncomingDate = DateTime.Now; });
                }
                else
                {
                    context.ACRALegalMapperActivities.Add(new ACRALegalMapperActivity { ACRAID = ACRAID, OrganizationID = OrgID, isRemoved = false, ActionDate = DateTime.Now });
                    context.ACRALegalMappers.Add(new ACRALegalMapper { ACRAID = ACRAID, OrganizationID = OrgID, Status = 1, BANKID = BankID, StageID = (string.IsNullOrEmpty(BankID)) ? (sbyte)2 : (sbyte)1, IncomingDate = DateTime.Now });
                }



                context.ACRAPersonMapperActivities.Add(new ACRAPersonMapperActivity { ACRAID = (int)ACRAID, PersonID = PersonID, isRemoved = false, ActionDate = DateTime.Now });
                    context.ACRAPersonMappers.Add(new ACRAPersonMapper { ACRAID = (int)ACRAID, PersonID = PersonID, Status = 1, StageID = 2, IncomingDate = DateTime.Now, BANKID = string.Empty });
                
                else
                {
                    context.ACRAPersonMapperActivities.Add(new ACRAPersonMapperActivity { ACRAID = (int)ACRAID, PersonID = PersonID, isRemoved = false, ActionDate = DateTime.Now });

                    context.ACRAPersonMappers.Add(new ACRAPersonMapper { ACRAID = (int)ACRAID, PersonID = PersonID, Status = 1, StageID = 2, IncomingDate = DateTime.Now, BANKID = string.Empty });
                }




                var _legalMapper = context.ACRALegalMappers.Where(m => m.OrganizationID == OrgID).Select(p => new { p.ACRAID, p.OrganizationID }).FirstOrDefault();

                if (_legalMapper != null)
                {
                    context.ACRALegalMapperActivities.Add(new ACRALegalMapperActivity { ACRAID = legalMapper.ACRAID, OrganizationID = OrgID, isRemoved = true, ActionDate = DateTime.Now });
                    context.ACRALegalMappers.Where(m => m.OrganizationID == OrgID).ForEachAsync(p => { p.ACRAID = ACRAID; p.BANKID = BankID; p.Status = 1; p.StageID = (string.IsNullOrEmpty(BankID)) ? (sbyte)2 : (sbyte)1; p.IncomingDate = DateTime.Now; });
                }
                else
                {
                    context.ACRALegalMapperActivities.Add(new ACRALegalMapperActivity { ACRAID = ACRAID, OrganizationID = OrgID, isRemoved = false, ActionDate = DateTime.Now });
                    context.ACRALegalMappers.Add(new ACRALegalMapper { ACRAID = ACRAID, OrganizationID = OrgID, Status = 1, BANKID = BankID, StageID = (string.IsNullOrEmpty(BankID)) ? (sbyte)2 : (sbyte)1, IncomingDate = DateTime.Now });
                }


                context.SaveChanges();
                _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
                return (int)ACRAID;
            }
        }
        */

        public int? ComputeACRAID(string ANTP, int OrgID, string BankID)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
            {
                int ACRAID = GenerateACRAID(ANTP);
                ACRALegalMapper legalMapper = new ACRALegalMapper();

                var _legalMapper = System.Linq.Queryable.Where(context.ACRALegalMappers, m => m.OrganizationID == OrgID).Select(p => new { p.ACRAID, p.OrganizationID }).FirstOrDefault();

                if (_legalMapper != null)
                {
                    //context.ACRALegalMapperActivities.Add(new ACRALegalMapperActivity { ACRAID = legalMapper.ACRAID, OrganizationID = OrgID, isRemoved = true, ActionDate = DateTime.Now });
                    var mapper = System.Linq.Queryable.Where(context.ACRALegalMappers, m => m.OrganizationID == OrgID).First();
                    mapper.ACRAID = ACRAID;
                    mapper.BANKID = BankID;
                    mapper.StageID = (string.IsNullOrEmpty(BankID)) ? 2 : 1;
                    mapper.IncomingDate = DateTime.Now;
                    mapper.Status = 1;
                    context.ACRALegalMappers.Update(mapper);
                    //context.ACRALegalMappers.Where(m => m.OrganizationID == OrgID).ForEachAsync(p => { p.ACRAID = ACRAID; p.BANKID = BankID; p.Status = 1; p.StageID = (string.IsNullOrEmpty(BankID)) ? (sbyte)2 : (sbyte)1; p.IncomingDate = DateTime.Now; });                  

                }
                else
                {
                    //context.ACRALegalMapperActivities.Add(new ACRALegalMapperActivity { ACRAID = ACRAID, OrganizationID = OrgID, isRemoved = false, ActionDate = DateTime.Now });
                    context.ACRALegalMappers.Add(new ACRALegalMapper { ACRAID = ACRAID, OrganizationID = OrgID, Status = 1, BANKID = BankID, StageID = (string.IsNullOrEmpty(BankID)) ? (sbyte)2 : (sbyte)1, IncomingDate = DateTime.Now });
                }

                context.SaveChanges();
                _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
                return ACRAID;
            }
        }

        public override int? ComputeACRAID(string IDNum, int EntityID)
        {
            throw new NotImplementedException();
        }
    }
}
