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
    public class AcraIdentityValidatorBankIDModel_Person : Person_AcraIdentityValidatorModel
    {
        AVVMapper _avvMapper;
        private Logger _logger;
        DbContextOptions<Acra3DbContext> _acra3DbOptions;
        DbContextOptions<Acra4DbContext> _acra4DbOptions;
        public static List<BankIDs> BankIDs;
        public static List<ACRAIdentity> ACRAIDs;

        private Models.AVV.AvvResponse responseModel = new Models.AVV.AvvResponse();

        public AcraIdentityValidatorBankIDModel_Person(DbContextOptions<Acra3DbContext> acra3dbOptions, DbContextOptions<Acra4DbContext> acra4dbOptions, Logger logger) : base(acra3dbOptions, acra4dbOptions, logger)
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
            //string SSN = GetIDNumFromACRA(EntityID);
            //if (!string.IsNullOrEmpty(SSN))
            //    return AcraIdentityValidator(SSN, EntityID);
            //else
            //{
            //    SSN = GetIDNumFrom3rdSource(EntityID);
            //    if (!string.IsNullOrEmpty(SSN))
            //        return AcraIdentityValidator(SSN, EntityID);
            //    else
            //        return null;
            //}
        }

        public override int? AcraIdentityValidator(int EntityID)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");

            List<PersonInfo> personInfo = GetPersonInfoUTF8(EntityID);
            if (personInfo == null) return null; 
            
            AcraData.Models.Acra3.BankIDs bankIDs = new BankIDs();
            int index = 0;
            using (Acra3DbContext context = new Acra3DbContext(_acra3DbOptions))
            {
                //if (!string.IsNullOrEmpty(personInfo.First().SocialCard))
                //{
                //    // 1. Check FName, LName, SSN
                //    bankIDs = BankIDs.Where(p => personInfo.First().FirstName.Replace("-", " ") == p.FirstName.Replace("-", " ")
                //                                        && personInfo.First().LastName.Replace("-", " ") == p.LastName.Replace("-", " ")
                //                                        && (personInfo.First().SocialCard == p.SocialCard || personInfo.First().SocialCard == p.HasNSocialCard)).FirstOrDefault();

                //    if (bankIDs != null)
                //        return ComputeACRAID(personInfo.First().SocialCard, EntityID, bankIDs.BankID);

                //    // 2. Check SSN, DocumentNum, FName OR LName
                //    index = 0;
                //    while (index < personInfo.Count)
                //    {
                //        // 2.1 Check SSN, DocumentNum, FName
                //        bankIDs = BankIDs.Where(p => personInfo[index].FirstName.Replace("-", " ") == p.FirstName.Replace("-", " ")
                //                                        && (personInfo[index].SocialCard == p.SocialCard || personInfo[index].SocialCard == p.HasNSocialCard)
                //                                        && personInfo[index].DocumentNum == p.PassportNum.Trim()).FirstOrDefault();
                //        if (bankIDs != null)
                //            return ComputeACRAID(personInfo.First().SocialCard, EntityID, bankIDs.BankID);

                //        // 2.2 Check SSN, DocumentNum, LName
                //        bankIDs = BankIDs.Where(p => personInfo[index].LastName.Replace("-", " ") == p.LastName.Replace("-", " ")
                //                                       && (personInfo[index].SocialCard == p.SocialCard || personInfo[index].SocialCard == p.HasNSocialCard)
                //                                       && personInfo[index].DocumentNum == p.PassportNum.Trim()).FirstOrDefault();
                //        if (bankIDs != null)
                //            return ComputeACRAID(personInfo.First().SocialCard, EntityID, bankIDs.BankID);

                //        index++;
                //    }
                //}

                //// 3. Check DocumentNum, FName, LName
                //index = 0;
                //while (index < personInfo.Count)
                //{                    
                //    bankIDs = BankIDs.Where(p => personInfo[index].FirstName.Replace("-", " ") == p.FirstName.Replace("-", " ")
                //                                    && personInfo[index].LastName.Replace("-", " ") == p.LastName.Replace("-", " ")
                //                                    && personInfo[index].DocumentNum == p.PassportNum.Trim()).FirstOrDefault();
                //    if (bankIDs != null)
                //        return ComputeACRAID(personInfo.First().SocialCard, EntityID, bankIDs.BankID);

                //    index++;
                //}

                ////// 4. Check FName, LName, BirthDate
                ////bankIDs = BankIDs.Where(p => personInfo.First().FirstName.Replace("-", " ") == p.FirstName.Replace("-", " ")
                ////                                && personInfo.First().LastName.Replace("-", " ") == p.LastName.Replace("-", " ")
                ////                                && personInfo.First().BirthDate == p.BirthDate).FirstOrDefault();
                ////if (bankIDs != null)
                ////    return ComputeACRAID(personInfo.First().SocialCard, EntityID, bankIDs.BankID);
                ////else
                return ComputeScorePerson(personInfo);
            }               
        }

        //public int ComputeScorePerson(List<PersonInfo> personInfo)
        //{
        //    int ScorePersonID = 0;
        //    using (Acra4DbContext context = new Acra4DbContext(_acra4DbOptions))
        //    {
        //        var NoBankIdPersons = context.ACRAPersonMappers.Where(p => string.IsNullOrEmpty(p.BANKID)).Select(p => new { p.PersonID, p.ACRAID }).ToList();
        //        if (NoBankIdPersons != null)
        //        {
        //            int index = 0;
        //            PersonInfo scorePerson = new PersonInfo();
        //            NoBankIdPersons.AsParallel().ForAll(noBankIdPerson =>
        //            {
        //                List<PersonInfo> noBankIdPersonInfo = GetPersonInfoUTF8(noBankIdPerson.PersonID);
        //                if (!string.IsNullOrEmpty(personInfo.First().SocialCard)
        //                    && !string.IsNullOrEmpty(noBankIdPersonInfo.First().SocialCard))
        //                {
        //                    // 1. Check FName, LName, SSN
        //                    if (personInfo.First().SocialCard == noBankIdPersonInfo.First().SocialCard
        //                        && personInfo.First().FirstName.Replace("-", " ") == noBankIdPersonInfo.First().FirstName.Replace("-", " ")
        //                        && personInfo.First().LastName.Replace("-", " ") == noBankIdPersonInfo.First().LastName.Replace("-", " "))

        //                        ScorePersonID = ComputeScorePersonID(personInfo.First().PersonId, noBankIdPerson.ACRAID);
        //                    if (ScorePersonID == 0)
        //                    {
        //                        // 2. Check SSN, DocumentNum, FName OR LName
        //                        index = 0;
        //                        while (index < personInfo.Count && ScorePersonID == 0)
        //                        {
        //                            // 2.1 Check SSN, DocumentNum, FName
        //                            scorePerson = noBankIdPersonInfo.Where(p => personInfo[index].FirstName.Replace("-", " ") == p.FirstName.Replace("-", " ")
        //                                                            && personInfo[index].SocialCard == p.SocialCard
        //                                                            && personInfo[index].DocumentNum == p.DocumentNum).FirstOrDefault();
        //                            if (scorePerson != null)
        //                                ScorePersonID = ComputeScorePersonID(personInfo.First().PersonId, noBankIdPerson.ACRAID);

        //                            // 2.2 Check SSN, DocumentNum, LName
        //                            scorePerson = noBankIdPersonInfo.Where(p => personInfo[index].LastName.Replace("-", " ") == p.LastName.Replace("-", " ")
        //                                                           && personInfo[index].SocialCard == p.SocialCard
        //                                                           && personInfo[index].DocumentNum == p.DocumentNum).FirstOrDefault();
        //                            if (scorePerson != null)
        //                                ScorePersonID = ComputeScorePersonID(personInfo.First().PersonId, noBankIdPerson.ACRAID);

        //                            index++;
        //                        }
        //                    }
        //                }

        //                // 3. Check DocumentNum, FName, LName
        //                if (ScorePersonID == 0)
        //                {
        //                    index = 0;
        //                    while (index < personInfo.Count && ScorePersonID == 0)
        //                    {
        //                        scorePerson = noBankIdPersonInfo.Where(p => personInfo[index].FirstName.Replace("-", " ") == p.FirstName.Replace("-", " ")
        //                                                        && personInfo[index].LastName.Replace("-", " ") == p.LastName.Replace("-", " ")
        //                                                        && personInfo[index].DocumentNum == p.DocumentNum).FirstOrDefault();
        //                        if (scorePerson != null)
        //                            ScorePersonID = ComputeScorePersonID(personInfo.First().PersonId, noBankIdPerson.ACRAID);

        //                        index++;
        //                    }
        //                }
        //                if (ScorePersonID == 0)
        //                {
        //                    // 4. Check FName, LName, BirthDate
        //                    if (!personInfo.First().BirthDate.ToString().Equals("0000-00-00")
        //                    && !noBankIdPersonInfo.First().BirthDate.ToString().Equals("0000-00-00"))
        //                    {
        //                        scorePerson = noBankIdPersonInfo.Where(p => personInfo.First().FirstName.Replace("-", " ") == p.FirstName.Replace("-", " ")
        //                                                    && personInfo.First().LastName.Replace("-", " ") == p.LastName.Replace("-", " ")
        //                                                    && personInfo.First().BirthDate == p.BirthDate).FirstOrDefault();
        //                        if (scorePerson != null)
        //                            ScorePersonID = ComputeScorePersonID(personInfo.First().PersonId, noBankIdPerson.ACRAID);
        //                    }
        //                }

        //                if (ScorePersonID == 0)
        //                    ScorePersonID = ComputeScorePersonID(personInfo.First().PersonId, null);
        //            });                                      
        //        }

        //    }
        //    return ScorePersonID;
        //}

        //public int ComputeScorePerson(List<PersonInfo> personInfo)
        //{
        //    int ScorePersonID = 0;
        //    using (Acra4DbContext context = new Acra4DbContext(_acra4DbOptions))
        //    {
        //        var NoBankIdPersons = context.ACRAPersonMappers.Where(p => string.IsNullOrEmpty(p.BANKID)).Select(p => new { p.PersonID, p.ACRAID }).ToList();
        //        if (NoBankIdPersons != null)
        //        {
        //            int index = 0;
        //            PersonInfo scorePerson = new PersonInfo();
        //            List<PersonInfo> noBankIdPersonInfo = GetPersonsInfoUTF8(string.Join(", ", NoBankIdPersons.Select(p=>p.PersonID).ToList()));

        //            foreach (var noBankIdPerson in NoBankIdPersons)
        //            {

        //                // 1. Check FName, LName, SSN
        //                if (!string.IsNullOrEmpty(personInfo.First().SocialCard)
        //                    && !string.IsNullOrEmpty(noBankIdPersonInfo.First().SocialCard))
        //                {
        //                    if (personInfo.First().SocialCard == noBankIdPersonInfo.First().SocialCard
        //                        && personInfo.First().FirstName.Replace("-", " ") == noBankIdPersonInfo.First().FirstName.Replace("-", " ")
        //                        && personInfo.First().LastName.Replace("-", " ") == noBankIdPersonInfo.First().LastName.Replace("-", " "))
        //                        return ComputeScorePersonID(personInfo.First().PersonId, noBankIdPerson.ACRAID);
        //                    // 2. Check SSN, DocumentNum, FName OR LName
        //                    index = 0;
        //                    while (index < personInfo.Count)
        //                    {
        //                        // 2.1 Check SSN, DocumentNum, FName
        //                        scorePerson = noBankIdPersonInfo.Where(p => personInfo[index].FirstName.Replace("-", " ") == p.FirstName.Replace("-", " ")
        //                                                        && personInfo[index].SocialCard == p.SocialCard
        //                                                        && personInfo[index].DocumentNum == p.DocumentNum).FirstOrDefault();
        //                        if (scorePerson != null)
        //                            return ComputeScorePersonID(personInfo.First().PersonId, noBankIdPerson.ACRAID);

        //                        // 2.2 Check SSN, DocumentNum, LName
        //                        scorePerson = noBankIdPersonInfo.Where(p => personInfo[index].LastName.Replace("-", " ") == p.LastName.Replace("-", " ")
        //                                                       && personInfo[index].SocialCard == p.SocialCard
        //                                                       && personInfo[index].DocumentNum == p.DocumentNum).FirstOrDefault();
        //                        if (scorePerson != null)
        //                            return ComputeScorePersonID(personInfo.First().PersonId, noBankIdPerson.ACRAID);

        //                        index++;
        //                    }
        //                }

        //                // 3. Check DocumentNum, FName, LName
        //                index = 0;
        //                while (index < personInfo.Count)
        //                {
        //                    scorePerson = noBankIdPersonInfo.Where(p => personInfo[index].FirstName.Replace("-", " ") == p.FirstName.Replace("-", " ")
        //                                                    && personInfo[index].LastName.Replace("-", " ") == p.LastName.Replace("-", " ")
        //                                                    && personInfo[index].DocumentNum == p.DocumentNum).FirstOrDefault();
        //                    if (scorePerson != null)
        //                        return ComputeScorePersonID(personInfo.First().PersonId, noBankIdPerson.ACRAID);

        //                    index++;
        //                }

        //                // 4. Check FName, LName, BirthDate
        //                if (!personInfo.First().BirthDate.ToString().Equals("0000-00-00")
        //                    && !noBankIdPersonInfo.First().BirthDate.ToString().Equals("0000-00-00"))
        //                {
        //                    scorePerson = noBankIdPersonInfo.Where(p => personInfo.First().FirstName.Replace("-", " ") == p.FirstName.Replace("-", " ")
        //                                                && personInfo.First().LastName.Replace("-", " ") == p.LastName.Replace("-", " ")
        //                                                && personInfo.First().BirthDate == p.BirthDate).FirstOrDefault();
        //                    if (scorePerson != null)
        //                        return ComputeScorePersonID(personInfo.First().PersonId, noBankIdPerson.ACRAID);
        //                }
        //            }
        //        }
        //        return ComputeScorePersonID(personInfo.First().PersonId, null);
        //    }
        //}



        public int ComputeScorePerson(List<PersonInfo> personInfo)
        {
            int ScorePersonID = 0;
            using (Acra4DbContext context = new Acra4DbContext(_acra4DbOptions))
            {
                var NoBankIdPersons = System.Linq.Queryable.Where(context.ACRAPersonMappers, p => string.IsNullOrEmpty(p.BANKID)).Select(p => new { p.PersonID, p.ACRAID }).ToList();
                if (NoBankIdPersons != null && NoBankIdPersons.Count > 0)
                {
                    int index = 0;
                    PersonInfo scorePerson = new PersonInfo();
                    List<PersonInfo> noBankIdPersonInfo = GetPersonsInfoUTF8(string.Join(", ", NoBankIdPersons.Select(p => p.PersonID).ToList()));

                    //foreach (var noBankIdPerson in NoBankIdPersons)
                    //{

                        // 1. Check FName, LName, SSN
                        if (!string.IsNullOrEmpty(personInfo.First().SocialCard)
                            && !string.IsNullOrEmpty(noBankIdPersonInfo.First().SocialCard))
                        {
                            //if (personInfo.First().SocialCard == noBankIdPersonInfo.First().SocialCard
                            //    && personInfo.First().FirstName.Replace("-", " ") == noBankIdPersonInfo.First().FirstName.Replace("-", " ")
                            //    && personInfo.First().LastName.Replace("-", " ") == noBankIdPersonInfo.First().LastName.Replace("-", " "))
                            //    return ComputeScorePersonID(personInfo.First().PersonId, noBankIdPerson.ACRAID);

                            var person = noBankIdPersonInfo.Where(p => p.SocialCard == personInfo.First().SocialCard
                                        && personInfo.First().FirstName.Replace("-", string.Empty).Replace(" ",string.Empty) == p.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty)
                                        && personInfo.First().LastName.Replace("-", string.Empty).Replace(" ", string.Empty) == p.LastName.Replace("-", string.Empty).Replace(" ", string.Empty)).FirstOrDefault();
                            if(person != null)
                                return ComputeScorePersonID(personInfo.First().PersonId, System.Linq.Queryable.Where(context.ACRAPersonMappers, p => p.PersonID == person.PersonId).First().ACRAID);




                            // 2. Check SSN, DocumentNum, FName OR LName
                            index = 0;
                            while (index < personInfo.Count)
                            {
                                // 2.1 Check SSN, DocumentNum, FName
                                scorePerson = noBankIdPersonInfo.Where(p => personInfo[index].FirstName.Replace("-", string.Empty).Replace(" ", string.Empty) == p.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty)
                                                                && personInfo[index].SocialCard == p.SocialCard
                                                                && personInfo[index].DocumentNum == p.DocumentNum).FirstOrDefault();
                                if (scorePerson != null)
                                    return ComputeScorePersonID(personInfo.First().PersonId, System.Linq.Queryable.Where(context.ACRAPersonMappers, p => p.PersonID == scorePerson.PersonId).First().ACRAID);




                                // 2.2 Check SSN, DocumentNum, LName
                                scorePerson = noBankIdPersonInfo.Where(p => personInfo[index].LastName.Replace("-", string.Empty).Replace(" ", string.Empty) == p.LastName.Replace("-", string.Empty).Replace(" ", string.Empty)
                                                               && personInfo[index].SocialCard == p.SocialCard
                                                               && personInfo[index].DocumentNum == p.DocumentNum).FirstOrDefault();
                                if (scorePerson != null)
                                    return ComputeScorePersonID(personInfo.First().PersonId, System.Linq.Queryable.Where(context.ACRAPersonMappers, p => p.PersonID == scorePerson.PersonId).First().ACRAID);

                                index++;
                            }


                        }

                        // 3. Check DocumentNum, FName, LName
                        index = 0;
                        while (index < personInfo.Count)
                        {
                            scorePerson = noBankIdPersonInfo.Where(p => personInfo[index].FirstName.Replace("-", string.Empty).Replace(" ", string.Empty) == p.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty)
                                                            && personInfo[index].LastName.Replace("-", string.Empty).Replace(" ", string.Empty) == p.LastName.Replace("-", string.Empty).Replace(" ", string.Empty)
                                                            && personInfo[index].DocumentNum == p.DocumentNum).FirstOrDefault();
                            if (scorePerson != null)
                                return ComputeScorePersonID(personInfo.First().PersonId, System.Linq.Queryable.Where(context.ACRAPersonMappers, p => p.PersonID == scorePerson.PersonId).First().ACRAID);

                            index++;
                        }

                        // 4. Check FName, LName, BirthDate
                        if ( !string.IsNullOrEmpty(personInfo.First().BirthDate) 
                        && !string.IsNullOrEmpty(noBankIdPersonInfo.First().BirthDate)
                        && !personInfo.First().BirthDate.Contains("00")
                        && !noBankIdPersonInfo.First().BirthDate.Contains("00"))                      
                        {
                            scorePerson = noBankIdPersonInfo.Where(p => personInfo.First().FirstName.Replace("-", string.Empty).Replace(" ", string.Empty) == p.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty)
                                                        && personInfo.First().LastName.Replace("-", string.Empty).Replace(" ", string.Empty) == p.LastName.Replace("-", string.Empty).Replace(" ", string.Empty)
                                                        && personInfo.First().BirthDate == p.BirthDate).FirstOrDefault();
                            if (scorePerson != null)
                                return ComputeScorePersonID(personInfo.First().PersonId, System.Linq.Queryable.Where(context.ACRAPersonMappers, p => p.PersonID == scorePerson.PersonId).First().ACRAID);
                        }
                    //}
                }
                return ComputeScorePersonID(personInfo.First().PersonId, null);
            }
        }
        public override bool CheckAllDocsExistance(string IDNum, int EntityID)
        {
            throw new NotImplementedException();
        }

        public int GenerateACRAIDUsingBankID(string bankID)
        {
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
            {
                _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");

                var acraIdentity = System.Linq.Queryable.Where(context.ACRAIdentities, p => p.IsLegal == false && p.ACRAGroup == bankID).FirstOrDefault();
                if (acraIdentity != null)
                    return acraIdentity.ACRAID;
                else
                {
                    var maxIdentity = System.Linq.Queryable.Where(context.ACRAIdentities, p => p.ACRAID < 550000000).OrderByDescending(p => p.ACRAID).FirstOrDefault();
                    int ACRAID = 0;
                    if (maxIdentity == null)
                        ACRAID = 1;
                    else
                        ACRAID = maxIdentity.ACRAID + 1;

                    ACRAIdentity identity = new ACRAIdentity() { ACRAID = ACRAID, ACRAGroup = bankID, IsLegal = false };
                    context.Add(identity);
                    context.SaveChanges();
                    return identity.ACRAID;
                }
            }
        }

        public override int GenerateACRAID(string IDNum)
        {
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
            {
                _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");

                var acraIdentity = System.Linq.Queryable.Where(context.ACRAIdentities, p => p.IsLegal == false && p.ACRAGroup == IDNum).FirstOrDefault();
                if (acraIdentity != null)
                    return acraIdentity.ACRAID;
                else
                {
                    var maxIdentity = System.Linq.Queryable.Where(context.ACRAIdentities, p => p.ACRAID < 550000000).OrderByDescending(p => p.ACRAID).FirstOrDefault();
                    int ACRAID = 0;
                    if (maxIdentity == null)
                        ACRAID = 1;
                    else
                        ACRAID = maxIdentity.ACRAID + 1;

                    ACRAIdentity identity = new ACRAIdentity() { ACRAID = ACRAID, ACRAGroup = IDNum, IsLegal = false };
                    context.Add(identity);
                    context.SaveChanges();
                    return identity.ACRAID;
                }
            }
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

        public List<PersonInfo> GetPersonInfoUTF8(int PersonId)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using (var IdentDB = new Acra3DbContext(_acra3DbOptions))
            {
                string sql = $" SELECT DISTINCT PersonId, SocialCard, BirthDate, FirstName, LastName, DocumentNum FROM(" +
                         $"SELECT Persons.PersonId, convert(cast(convert(Persons.SocialCard using  latin1) as binary) using utf8) as SocialCard, Persons.BirthDate, " +
                         $" convert(cast(convert(DicFirstNames.FirstName using  latin1) as binary) using utf8) as FirstName, " +
                         $" convert(cast(convert(DicLastNames.LastName using  latin1) as binary) using utf8) as LastName, " +
                         $" convert(cast(convert(Passports.PassportNum using  latin1) as binary) using utf8) as DocumentNum" +
                         $" FROM Persons " +
                         $" INNER JOIN SourceReference ON Persons.PersonId = SourceReference.RecordID " +
                         $" INNER JOIN DicFirstNames ON Persons.FirstName = DicFirstNames.FirstNameID " +
                         $" INNER JOIN DicLastNames ON Persons.LastName = DicLastNames.LastNameID " +
                         $" LEFT JOIN Passports ON(Passports.PersonId = Persons.PersonId) " +
                         $" INNER JOIN SourceReference AS ValidPassport ON(ValidPassport.ReferenceTable = 2 AND ValidPassport.Status = 1 AND Passports.PassportID = ValidPassport.RecordID) " +                        
                         $" WHERE  SourceReference.ReferenceTable = 1 AND SourceReference.Status = 1 AND Passports.PassportNum is not null AND Persons.PersonId IN({PersonId}) " +
                         $" UNION ALL"+
                         $"  SELECT Persons.PersonId, convert(cast(convert(Persons.SocialCard using  latin1) as binary) using utf8) as SocialCard, Persons.BirthDate, " +
                          $" convert(cast(convert(DicFirstNames.FirstName using  latin1) as binary) using utf8) as FirstName, " +
                         $" convert(cast(convert(DicLastNames.LastName using  latin1) as binary) using utf8) as LastName, " +
                         $" convert(cast(convert(IdCards.IdCardNum using  latin1) as binary) using utf8) as DocumentNum" +                         
                         $" FROM Persons " +
                         $" INNER JOIN SourceReference ON Persons.PersonId = SourceReference.RecordID " +
                         $" INNER JOIN DicFirstNames ON Persons.FirstName = DicFirstNames.FirstNameID " +
                         $" INNER JOIN DicLastNames ON Persons.LastName = DicLastNames.LastNameID " +
                         $" LEFT JOIN IdCards ON(IdCards.PersonId = Persons.PersonId) " +
                         $" INNER JOIN SourceReference AS SourceRef ON(SourceRef.ReferenceTable = 7 AND SourceRef.Status = 1 AND IdCards.IdCardID = SourceRef.RecordID) " +                         
                         $" WHERE  SourceReference.ReferenceTable = 1 AND SourceReference.Status = 1 AND  IdCards.IdCardNum is not null AND Persons.PersonId IN({PersonId})) as Info" ;

                //return IdentDB.RawSqlQuery<List<PersonInfo>>(sql).FirstOrDefault();
                _logger.Log.Info($"SQL:{sql}");
                return IdentDB.RawSqlQuery<PersonInfo>(sql, p=> new PersonInfo
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

        public List<PersonInfo> GetPersonsInfoUTF8(string PersonIds)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using (var IdentDB = new Acra3DbContext(_acra3DbOptions))
            {
                string sql = $" SELECT DISTINCT PersonId, SocialCard, BirthDate, FirstName, LastName, DocumentNum FROM(" +
                         $"SELECT Persons.PersonId, convert(cast(convert(Persons.SocialCard using  latin1) as binary) using utf8) as SocialCard, Persons.BirthDate, " +
                         $" convert(cast(convert(DicFirstNames.FirstName using  latin1) as binary) using utf8) as FirstName, " +
                         $" convert(cast(convert(DicLastNames.LastName using  latin1) as binary) using utf8) as LastName, " +
                         $" convert(cast(convert(Passports.PassportNum using  latin1) as binary) using utf8) as DocumentNum" +
                         $" FROM Persons " +
                         $" INNER JOIN SourceReference ON Persons.PersonId = SourceReference.RecordID " +
                         $" INNER JOIN DicFirstNames ON Persons.FirstName = DicFirstNames.FirstNameID " +
                         $" INNER JOIN DicLastNames ON Persons.LastName = DicLastNames.LastNameID " +
                         $" LEFT JOIN Passports ON(Passports.PersonId = Persons.PersonId) " +
                         $" INNER JOIN SourceReference AS ValidPassport ON(ValidPassport.ReferenceTable = 2 AND ValidPassport.Status = 1 AND Passports.PassportID = ValidPassport.RecordID) " +
                         $" WHERE  SourceReference.ReferenceTable = 1 AND SourceReference.Status = 1 AND Passports.PassportNum is not null AND Persons.PersonId IN({PersonIds}) " +
                         $" UNION ALL" +
                         $"  SELECT Persons.PersonId, convert(cast(convert(Persons.SocialCard using  latin1) as binary) using utf8) as SocialCard, Persons.BirthDate, " +
                          $" convert(cast(convert(DicFirstNames.FirstName using  latin1) as binary) using utf8) as FirstName, " +
                         $" convert(cast(convert(DicLastNames.LastName using  latin1) as binary) using utf8) as LastName, " +
                         $" convert(cast(convert(IdCards.IdCardNum using  latin1) as binary) using utf8) as DocumentNum" +
                         $" FROM Persons " +
                         $" INNER JOIN SourceReference ON Persons.PersonId = SourceReference.RecordID " +
                         $" INNER JOIN DicFirstNames ON Persons.FirstName = DicFirstNames.FirstNameID " +
                         $" INNER JOIN DicLastNames ON Persons.LastName = DicLastNames.LastNameID " +
                         $" LEFT JOIN IdCards ON(IdCards.PersonId = Persons.PersonId) " +
                         $" INNER JOIN SourceReference AS SourceRef ON(SourceRef.ReferenceTable = 7 AND SourceRef.Status = 1 AND IdCards.IdCardID = SourceRef.RecordID) " +
                         $" WHERE  SourceReference.ReferenceTable = 1 AND SourceReference.Status = 1 AND  IdCards.IdCardNum is not null AND Persons.PersonId IN({PersonIds})) as Info";

                //return IdentDB.RawSqlQuery<List<PersonInfo>>(sql).FirstOrDefault();
                _logger.Log.Info($"SQL:{sql}");
                return IdentDB.RawSqlQuery<PersonInfo>(sql, p => new PersonInfo
                {
                    PersonId = Convert.ToInt32(p["PersonId"]),
                    SocialCard = p["SocialCard"].ToString(),
                    //BirthDate = Convert.ToDateTime(p["BirthDate"].ToString()),
                    BirthDate = p["BirthDate"].ToString(),//(p["BirthDate"].ToString() != null && p["BirthDate"].ToString().Contains("00")) ? Convert.ToDateTime(p["BirthDate"].ToString()) : default(DateTime),
                    FirstName = p["FirstName"].ToString(),
                    LastName = p["LastName"].ToString(),
                    DocumentNum = p["DocumentNum"].ToString()
                });
            }
        }

        public int ComputeScorePersonID(int PersonID, int? ACRAID)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
            {
                ACRAPersonMapper personMapper = new ACRAPersonMapper();
                if (ACRAID == null)                
                {
                    var scorePersons = System.Linq.Queryable.Where(context.ACRAPersonMappers, p => p.ACRAID >= 550000000 && p.ACRAID < 559999999).OrderByDescending(p => p.ACRAID).Select(p=>new { p.ACRAID }).FirstOrDefault();
                    if (scorePersons == null)
                        ACRAID = 550000001;
                    else
                        ACRAID = scorePersons.ACRAID + 1;
                }


                var _personMapper = System.Linq.Queryable.Where(context.ACRAPersonMappers, m => m.PersonID == PersonID).Select(p => new { p.PersonID, p.ACRAID }).FirstOrDefault();

                if (_personMapper != null)
                {
                   // context.ACRAPersonMapperActivities.Add(new ACRAPersonMapperActivity { ACRAID = _personMapper.ACRAID, PersonID = PersonID, isRemoved = true, ActionDate = DateTime.Now });
                    // context.ACRAPersonMappers.Where(m => m.PersonID == PersonID).ForEachAsync(p => { p.ACRAID = (int)ACRAID; p.BANKID = string.Empty; p.StageID = 2; p.IncomingDate = DateTime.Now; p.Status = 1; });

                    var mapper = System.Linq.Queryable.Where(context.ACRAPersonMappers, m => m.PersonID == PersonID).First();
                    mapper.ACRAID = (int)ACRAID;
                    mapper.BANKID = string.Empty;
                    mapper.StageID = 2;
                    mapper.IncomingDate = DateTime.Now;
                    mapper.Status = 1;
                    context.ACRAPersonMappers.Update(mapper);                    
                }
                else
                {
                    //context.ACRAPersonMapperActivities.Add(new ACRAPersonMapperActivity { ACRAID = (int)ACRAID, PersonID = PersonID, isRemoved = false, ActionDate = DateTime.Now });
                    context.ACRAPersonMappers.Add(new ACRAPersonMapper { ACRAID = (int)ACRAID, PersonID = PersonID, Status = 1, StageID = 2, IncomingDate = DateTime.Now, BANKID = string.Empty });
                }



                context.SaveChanges();
                _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
                return (int)ACRAID;
            }
        }
        
        public int? ComputeACRAID(string SSN, int PersonID, string BankID)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
            {

                //int ACRAID = GenerateACRAID(SSN);                
                int ACRAID = GenerateACRAIDUsingBankID(BankID);
                var _personMapper = System.Linq.Queryable.Where(context.ACRAPersonMappers, m => m.PersonID == PersonID).Select(p=> new { p.PersonID, p.ACRAID}).FirstOrDefault();

                if (_personMapper != null)
                {
                    context.ACRAPersonMapperActivities.Add(new ACRAPersonMapperActivity { ACRAID = _personMapper.ACRAID, PersonID = PersonID, isRemoved = true, ActionDate = DateTime.Now });
                    var mapper = System.Linq.Queryable.Where(context.ACRAPersonMappers, m => m.PersonID == PersonID).First();
                    mapper.ACRAID = ACRAID;
                    mapper.BANKID = BankID;
                    mapper.StageID = 1;
                    mapper.IncomingDate = DateTime.Now;
                    mapper.Status = 1;                    
                    context.ACRAPersonMappers.Update(mapper);
                }
                else
                {
                    context.ACRAPersonMapperActivities.Add(new ACRAPersonMapperActivity { ACRAID = ACRAID, PersonID = PersonID, isRemoved = false, ActionDate = DateTime.Now });
                    context.ACRAPersonMappers.Add(new ACRAPersonMapper { ACRAID = ACRAID, PersonID = PersonID, Status = 1, BANKID = BankID, StageID = 1, IncomingDate = DateTime.Now });
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




        public bool SetACRAIDusingBankID(List<MonitoringPlusActivityTemp> monitoringPlusActivities)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            bool result = false;

          
            List<ACRAPersonMapperActivity> acraPersonMapperActivities = new List<ACRAPersonMapperActivity>();
            List<ACRAPersonMapper> acraPersonsMapper = new List<ACRAPersonMapper>();
           // List<ACRAPersonMapper> acraPersonsMapper = new List<ACRAPersonMapper>();
            List<int> personIDs = new List<int>();

            using (var acra3DbContext = new AcraData.Data.Acra3DbContext(_acra3DbOptions))
            {
                using (var tx3 = acra3DbContext.Database.BeginTransaction())
                {
                    using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
                    {
                        using (var tx = context.Database.BeginTransaction())
                        {
                            try
                            {

                                context.ChangeTracker.AutoDetectChangesEnabled = false;
                                acra3DbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                                #region 1. Check FName, LName, SSN
                                var personsInfos = GetPersonsInfoUTF8(string.Join(", ", monitoringPlusActivities.Select(p => p.ActivityId).ToList())).ToList();

                                
                                var bankIDs = (from p in personsInfos
                                               from b in BankIDs                                           
                                               where !string.IsNullOrEmpty(p.SocialCard)
                                               && (p.SocialCard.Trim() == b.SocialCard.Trim() || p.SocialCard.Trim() == b.HasNSocialCard.Trim())
                                               && p.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper()
                                               && p.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper()
                                               && !string.IsNullOrEmpty(b.BankID)
                                               select new { p.PersonId, b.BankID }).Distinct().ToList();

                                var mappers = (from b in bankIDs
                                              from a in ACRAIDs
                                              where !string.IsNullOrEmpty(b.BankID) && b.BankID == a.ACRAGroup
                                              select new ACRAPersonMapper() { ACRAID = a.ACRAID, PersonID = b.PersonId, Status = 1, BANKID = b.BankID, StageID = 1, IncomingDate = DateTime.Now }).ToList();

                                
                                acraPersonsMapper.AddRange(mappers);
                                personIDs.AddRange(mappers.Select(p=>p.PersonID));
                             
                                monitoringPlusActivities.RemoveAll(p => p.ActivityType == 10 && mappers.Any(m => m.PersonID == p.ActivityId));

                              

                                #endregion

                                #region 2. Check SSN, DocumentNum, FName OR LName
                               

                                bankIDs = (from p in personsInfos.Where(p => !personIDs.Contains(p.PersonId))
                                           from b in BankIDs
                                               where !string.IsNullOrEmpty(p.SocialCard)
                                               && (p.SocialCard.Trim() == b.SocialCard.Trim() || p.SocialCard.Trim() == b.HasNSocialCard.Trim())
                                               && p.DocumentNum.Trim() == b.PassportNum.Trim()
                                               && (p.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper()
                                                   || p.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper())
                                               
                                           select new { p.PersonId, b.BankID }).Distinct().ToList();


                                mappers = (from b in bankIDs
                                              where !string.IsNullOrEmpty(b.BankID)
                                              select new ACRAPersonMapper() { ACRAID = ACRAIDs.Where(p => p.ACRAGroup == b.BankID).First().ACRAID, PersonID = b.PersonId, Status = 1, BANKID = b.BankID, StageID = 1, IncomingDate = DateTime.Now }).ToList();

                                acraPersonsMapper.AddRange(mappers);
                                personIDs.AddRange(mappers.Select(p => p.PersonID));
                               
                                monitoringPlusActivities.RemoveAll(p => p.ActivityType == 10 && mappers.Any(m => m.PersonID == p.ActivityId));
                             
                                #endregion

                                #region 3. DocumentNum, FName, LName
                             

                                bankIDs = (from p in personsInfos.Where(p => !personIDs.Contains(p.PersonId))
                                           from b in BankIDs
                                           where p.DocumentNum.Trim() == b.PassportNum.Trim()
                                           && p.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper()
                                           && p.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper()
                                           select new { p.PersonId, b.BankID }).Distinct().ToList();

                                mappers = (from b in bankIDs
                                          where !string.IsNullOrEmpty(b.BankID)
                                          select new ACRAPersonMapper() { ACRAID = ACRAIDs.Where(p => p.ACRAGroup == b.BankID).First().ACRAID, PersonID = b.PersonId, Status = 1, BANKID = b.BankID, StageID = 1, IncomingDate = DateTime.Now }).ToList();

                                acraPersonsMapper.AddRange(mappers);
                                personIDs.AddRange(mappers.Select(p => p.PersonID));
                               
                                monitoringPlusActivities.RemoveAll(p => p.ActivityType == 10 && mappers.Any(m => m.PersonID == p.ActivityId));
                               
                                #endregion

                                monitoringPlusActivities.ForEach(p => { p.Status = 0; });


                                var monitoringActivities = System.Linq.Queryable
     .Where(acra3DbContext.MonitoringPlusActivityTmps, p => p.ActivityType == 10 && p.Status == 1 && personIDs.Contains((int)p.ActivityId))
     .ToList();

                                monitoringActivities.ForEach(p => { p.Status = 200; });
                                acra3DbContext.MonitoringPlusActivityTmps.UpdateRange(monitoringActivities);
                               


                                //acra3DbContext.MonitoringPlusActivityTmps.UpdateRange(monitoringActivities);
                                acra3DbContext.MonitoringPlusActivityTmps.UpdateRange(monitoringPlusActivities);


                                foreach (var item in acraPersonsMapper)
                                {
                                    if (context.ACRAPersonMappers.Any(p => p.PersonID == item.PersonID))
                                        context.ACRAPersonMappers.Update(item);
                                    else
                                        context.ACRAPersonMappers.Add(item);
                                }

                                // context.ACRAPersonMappers.AddRange(acraPersonsMapper);

                                acra3DbContext.SaveChanges();
                                context.SaveChanges();
                                tx.Commit();
                                tx3.Commit();
                                result = true;
                            }
                            catch (Exception ex)
                            {
                                _logger.Log.ErrorFormat($"{MethodBase.GetCurrentMethod().Name} failed monitoringPlusActivities Error:{ex.Message}");
                                tx.Rollback();
                                tx3.Rollback();
                            }
                            finally { context.SaveChanges(); acra3DbContext.SaveChanges(); }
                            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
                            return result;
                        }
                    }
                }
            }
        }
        /*
        public bool SetACRAIDusingBankIDParallel(List<MonitoringPlusActivityTemp> monitoringPlusActivities)
        {
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            bool result = false;


            List<ACRAPersonMapperActivity> acraPersonMapperActivities = new List<ACRAPersonMapperActivity>();
            List<ACRAPersonMapper> acraPersonsMapper = new List<ACRAPersonMapper>();
            // List<ACRAPersonMapper> acraPersonsMapper = new List<ACRAPersonMapper>();
            List<int> personIDs = new List<int>();

            using (var acra3DbContext = new AcraData.Data.Acra3DbContext(_acra3DbOptions))
            {
                using (var tx3 = acra3DbContext.Database.BeginTransaction())
                {
                    using (var context = new AcraData.Data.Acra4DbContext(_acra4DbOptions))
                    {
                        using (var tx = context.Database.BeginTransaction())
                        {
                            try
                            {
                                context.ChangeTracker.AutoDetectChangesEnabled = false;
                                acra3DbContext.ChangeTracker.AutoDetectChangesEnabled = false;
                                var bankIDs = new List<int, string>(){ PersonId = 0, BankID = string.Empty };
                                var mappers = new ACRAPersonMapper();
                                var personsInfos = GetPersonsInfoUTF8(string.Join(", ", monitoringPlusActivities.Select(p => p.ActivityId).ToList())).ToList();
                                monitoringPlusActivities.AsParallel().ForAll(monitoringPlusActivity =>
                                {
                                    if (personsInfos.Any(p => p.PersonId == monitoringPlusActivity.ActivityId && !string.IsNullOrEmpty(p.SocialCard)))
                                    {

                                        #region 1. Check FName, LName, SSN

                                        bankIDs = (from p in personsInfos
                                                       from b in BankIDs
                                                       where !string.IsNullOrEmpty(p.SocialCard)
                                                       && (p.SocialCard.Trim() == b.SocialCard.Trim() || p.SocialCard.Trim() == b.HasNSocialCard.Trim())
                                                       && p.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper()
                                                       && p.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper()
                                                       && !string.IsNullOrEmpty(b.BankID)
                                                       select new { p.PersonId, b.BankID }).Distinct().ToList();

                                        mappers = (from b in bankIDs
                                                       from a in ACRAIDs
                                                       where !string.IsNullOrEmpty(b.BankID) && b.BankID == a.ACRAGroup
                                                       select new ACRAPersonMapper() { ACRAID = a.ACRAID, PersonID = b.PersonId, Status = 1, BANKID = b.BankID, StageID = 1, IncomingDate = DateTime.Now }).ToList();


                                        acraPersonsMapper.AddRange(mappers);
                                        personIDs.AddRange(mappers.Select(p => p.PersonID));

                                        monitoringPlusActivities.RemoveAll(p => p.ActivityType == 10 && mappers.Any(m => m.PersonID == p.ActivityId));



                                        #endregion

                                        #region 2. Check SSN, DocumentNum, FName OR LName


                                        bankIDs = (from p in personsInfos.Where(p => !personIDs.Contains(p.PersonId))
                                                   from b in BankIDs
                                                   where !string.IsNullOrEmpty(p.SocialCard)
                                                   && (p.SocialCard.Trim() == b.SocialCard.Trim() || p.SocialCard.Trim() == b.HasNSocialCard.Trim())
                                                   && p.DocumentNum.Trim() == b.PassportNum.Trim()
                                                   && (p.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper()
                                                       || p.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper())

                                                   select new { p.PersonId, b.BankID }).Distinct().ToList();


                                        mappers = (from b in bankIDs
                                                   where !string.IsNullOrEmpty(b.BankID)
                                                   select new ACRAPersonMapper() { ACRAID = ACRAIDs.Where(p => p.ACRAGroup == b.BankID).First().ACRAID, PersonID = b.PersonId, Status = 1, BANKID = b.BankID, StageID = 1, IncomingDate = DateTime.Now }).ToList();

                                        acraPersonsMapper.AddRange(mappers);
                                        personIDs.AddRange(mappers.Select(p => p.PersonID));

                                        monitoringPlusActivities.RemoveAll(p => p.ActivityType == 10 && mappers.Any(m => m.PersonID == p.ActivityId));

                                        #endregion
                                    }

                                    #region 3. DocumentNum, FName, LName


                                    bankIDs = (from p in personsInfos.Where(p => !personIDs.Contains(p.PersonId))
                                               from b in BankIDs
                                               where p.DocumentNum.Trim() == b.PassportNum.Trim()
                                               && p.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.FirstName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper()
                                               && p.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper() == b.LastName.Replace("-", string.Empty).Replace(" ", string.Empty).ToUpper()
                                               select new { p.PersonId, b.BankID }).Distinct().ToList();

                                    mappers = (from b in bankIDs
                                               where !string.IsNullOrEmpty(b.BankID)
                                               select new ACRAPersonMapper() { ACRAID = ACRAIDs.Where(p => p.ACRAGroup == b.BankID).First().ACRAID, PersonID = b.PersonId, Status = 1, BANKID = b.BankID, StageID = 1, IncomingDate = DateTime.Now }).ToList();

                                    acraPersonsMapper.AddRange(mappers);
                                    personIDs.AddRange(mappers.Select(p => p.PersonID));

                                    monitoringPlusActivities.RemoveAll(p => p.ActivityType == 10 && mappers.Any(m => m.PersonID == p.ActivityId));

                                    #endregion

                                    monitoringPlusActivities.ForEach(p => { p.Status = 0; });


                                    var monitoringActivities = acra3DbContext.MonitoringPlusActivityTmps.Where(p => p.ActivityType == 10 && p.Status == 1 && personIDs.Contains((int)p.ActivityId)).ToList();
                                    monitoringActivities.ForEach(p => { p.Status = 200; });
                                    acra3DbContext.MonitoringPlusActivityTmps.UpdateRange(monitoringActivities);



                                    //acra3DbContext.MonitoringPlusActivityTmps.UpdateRange(monitoringActivities);
                                    acra3DbContext.MonitoringPlusActivityTmps.UpdateRange(monitoringPlusActivities);


                                    foreach (var item in acraPersonsMapper)
                                    {
                                        if (context.ACRAPersonMappers.Any(p => p.PersonID == item.PersonID))
                                            context.ACRAPersonMappers.Update(item);
                                        else
                                            context.ACRAPersonMappers.Add(item);
                                    }

                                    // context.ACRAPersonMappers.AddRange(acraPersonsMapper);

                                    acra3DbContext.SaveChanges();
                                    context.SaveChanges();
                                    tx.Commit();
                                    tx3.Commit();
                                    result = true;
                                }
                                );
                            }
                            catch (Exception ex)
                            {
                                _logger.Log.ErrorFormat($"{MethodBase.GetCurrentMethod().Name} failed monitoringPlusActivities Error:{ex.Message}");
                                tx.Rollback();
                                tx3.Rollback();
                            }
                            finally { context.SaveChanges(); acra3DbContext.SaveChanges(); }
                            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
                            return result;
                        }
                    }
                }
            }
        }
        */
    }
}
