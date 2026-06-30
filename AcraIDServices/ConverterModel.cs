using AcraData.Data;
using AcraData.Models.Acra4;
using AcraIDServices.Mappers;
using AcraIDServices.Models;
using AcraUtils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AcraIDServices
{
    public class ConverterModel
    {
        public static List<Models.PersonInfo> PersonsInfos = new List<Models.PersonInfo>();

        AVVMapper _avvMapper;
        private Logger _logger;
        private AcraUtils.Configuration.ValidatorConfig _configuration;


        DbContextOptions<Acra4DbContext> _acra4DbOptions;
        DbContextOptions<Acra3DbContext> _acra3DbOptions;
        DbContextOptions<AcraJournalDbContext> _acraJournalOptions;

        private Models.AVV.AvvResponse responseModel = new Models.AVV.AvvResponse();
        private Models.AVV.AvvResponse ssnResponseModel = new Models.AVV.AvvResponse();

        private bool isEkengSuccess = true;

        List<int> verifyInfoSuccesses = new List<int>();
        int verifyInfoSuccessCounter = 0;
        bool isEkengSuccessCollectInfo = false;

        public ConverterModel(DbContextOptions<Acra4DbContext> acra4dbOptions, Logger logger, IOptions<AcraUtils.Configuration.ValidatorConfig> configuration, DbContextOptions<Acra3DbContext> acra3dbOptions, DbContextOptions<AcraJournalDbContext> acraJournalOptions)
        {

            _acra4DbOptions = acra4dbOptions;
            _acraJournalOptions = acraJournalOptions;
            _logger = logger;
            _configuration = configuration.Value;
            _avvMapper = new AVVMapper(_logger, acra4dbOptions);
            _acra3DbOptions = acra3dbOptions;
        }

        public ConverterRespModel VerifyInfo(ConverterPersonInfo PersonInfo)
        {

            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   1" + Environment.NewLine);
            if (!PersonInfo.SocialCard.Check)
            {
                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   2" + Environment.NewLine);
                return new ConverterRespModel() { Status = 0 };
            }
            ////////////4.2.5
            if (PersonInfo.Residency.Value == "2" && PersonInfo.SocialCard.Value == "")
            {
                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   4" + Environment.NewLine);
                return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.MissingSocialCard } };
            }
            ////////////4.2.8
            if (PersonInfo.SocialCard.Value != "" && !long.TryParse(PersonInfo.SocialCard.Value, out long ssnIndicator))
            {
                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   3" + Environment.NewLine);
                if (SSNIndicatorExistanceIn3rdSource(PersonInfo.SocialCard.Value))
                {
                    AcraData.Models.Acra4.BPR_Persons aVVPerson = Get3rdSourceInfoFromDBBySsnIndicator(PersonInfo.SocialCard.Value);
                    PersonInfo.SocialCard.Value = aVVPerson.PNum;
                }
                else
                {
                    Get3rdSourceInfoBySSN(PersonInfo.SocialCard.Value);
                    if (isEkengSuccess == false)
                    {
                        return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.Ekeng } };
                    }
                    else if (responseModel != null && responseModel.Result != null && responseModel.Result.Count > 0)
                    {
                        _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                    }
                    if (SSNIndicatorExistanceIn3rdSource(PersonInfo.SocialCard.Value))
                    {
                        AcraData.Models.Acra4.BPR_Persons aVVPerson = Get3rdSourceInfoFromDBBySsnIndicator(PersonInfo.SocialCard.Value);
                        PersonInfo.SocialCard.Value = aVVPerson.PNum;
                    }
                }
            }
            ////////////4.2.6
            if (PersonInfo.Residency.Value == "2" && PersonInfo.SocialCard.Value != "")
            {
                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   5" + Environment.NewLine);
                if (!SSNExistanceIn3rdSource(PersonInfo.SocialCard.Value))
                {
                    //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   6" + Environment.NewLine);
                    Get3rdSourceInfoBySSN(PersonInfo.SocialCard.Value);
                    if (isEkengSuccess == false)
                    {
                        //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   9" + Environment.NewLine);
                        return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.Ekeng } };
                    }
                    if (responseModel == null || responseModel.Result == null)
                    {
                        //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   10" + Environment.NewLine);
                        return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.SocialCard } };
                    }
                    else if (responseModel != null && responseModel.Result != null && responseModel.Result.Count > 0)
                    {
                        //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   11" + Environment.NewLine);
                        _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                    }

                }
                ////////4.2.6.2 
                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   20" + Environment.NewLine);
                AcraData.Models.Acra4.BPR_Persons avvPerson = Get3rdSourceInfoFromDBBySSN(PersonInfo.SocialCard.Value);
                if (GetAge(avvPerson.BirthDate.Value) < 16)
                {
                    return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.BirthDate } };
                }
                if (ReplaceEV(PersonInfo.FirstName.Value).ToUpper() == ReplaceEV(avvPerson.FirstName).ToUpper() && ReplaceEV(PersonInfo.LastName.Value).ToUpper() == ReplaceEV(avvPerson.LastName).ToUpper())
                {
                    AddToDicsIfNotExist(PersonInfo.FirstName.Value, PersonInfo.LastName.Value);
                    return new ConverterRespModel() { Status = 0 };
                }
                else
                {
                    Get3rdSourceInfoBySSN(PersonInfo.SocialCard.Value);
                    if (isEkengSuccess == false)
                    {
                        return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.Ekeng } };
                    }
                    if (responseModel != null && responseModel.Result != null && responseModel.Result.Count == 1)
                    {
                        _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());

                        //var lastDocument = responseModel.Result.FirstOrDefault().AvvDocuments.Document.Where(p => p.PassportData != null).OrderByDescending(d => DateTime.ParseExact(d.PassportData.PassportIssuanceDate,"dd/mm/yyyy",null)).First();
                        avvPerson = Get3rdSourceInfoFromDBBySSN(PersonInfo.SocialCard.Value);
                        if (ReplaceEV(PersonInfo.FirstName.Value).ToUpper() == ReplaceEV(avvPerson.FirstName).ToUpper() && ReplaceEV(PersonInfo.LastName.Value).ToUpper() == ReplaceEV(avvPerson.LastName).ToUpper())
                        {
                            AddToDicsIfNotExist(PersonInfo.FirstName.Value, PersonInfo.LastName.Value);
                            return new ConverterRespModel() { Status = 0 };
                        }
                        else
                        {
                            if (ReplaceEV(PersonInfo.FirstName.Value).ToUpper() != ReplaceEV(avvPerson.FirstName).ToUpper() && ReplaceEV(PersonInfo.LastName.Value).ToUpper() != ReplaceEV(avvPerson.LastName).ToUpper())
                                return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.FirstName, ErrorType.LastName } };
                            if (ReplaceEV(PersonInfo.FirstName.Value).ToUpper() != ReplaceEV(avvPerson.FirstName).ToUpper())
                                return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.FirstName } };
                            if (ReplaceEV(PersonInfo.LastName.Value).ToUpper() != ReplaceEV(avvPerson.LastName).ToUpper())
                                return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.LastName } };
                        }
                    }
                    else
                    {
                        return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType> { ErrorType.Ekeng } };
                    }
                }
            }
            //////////4.2.7
            if (PersonInfo.Residency.Value != "2" && PersonInfo.SocialCard.Value != "")
            {
                return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.Residency } };
            }
            /////////4.2.9
            bool docExists = false;
            if (PersonInfo.Residency.Value != "2" && PersonInfo.SocialCard.Value == "")
            {
                foreach (var personDoc in PersonInfo.DocumentNum.Value)
                {
                    if (DocExistanceIn3rdSource(personDoc))
                    {
                        docExists = true;
                    }
                    else
                    {
                        Get3rdSourceInfoByDoc(personDoc);
                        if (isEkengSuccess == false)
                        {
                            return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.Ekeng } };
                        }
                        if (responseModel != null && responseModel.Result != null && responseModel.Result.Count == 1)
                        {
                            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   11" + Environment.NewLine);
                            _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                        }
                        if (DocExistanceIn3rdSource(personDoc))
                        {
                            docExists = true;
                        }
                    }
                }
                if (!docExists)
                {
                    int age;
                    try
                    {
                        age = GetAge(DateTime.ParseExact(PersonInfo.BirthDate.Value.ToString(), "yyyy/MM/dd", null));
                    }
                    catch (Exception)
                    {
                        try
                        {
                            age = GetAge(DateTime.ParseExact(PersonInfo.BirthDate.Value.ToString(), "yyyy/MM/DD", null));
                        }
                        catch (Exception)
                        {
                            try
                            {
                                age = GetAge(DateTime.ParseExact(PersonInfo.BirthDate.Value.ToString().Replace("-", "/"), "yyyy/MM/dd", null));
                            }
                            catch (Exception)
                            {
                                age = GetAge(DateTime.ParseExact(PersonInfo.BirthDate.Value.ToString().Replace("-", "/"), "yyyy/MM/DD", null));

                            }
                        }
                    }

                    if (age < 16)
                    {
                        return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.BirthDate } };
                    }
                    //AddToDicsIfNotExist(PersonInfo.FirstName.Value, PersonInfo.LastName.Value);
                    return new ConverterRespModel() { Status = 0 };
                }
                else
                {
                    foreach (var personDoc in PersonInfo.DocumentNum.Value)
                    {
                        AcraData.Models.Acra4.BPR_Documents avvDocument = Get3rdSourceInfoFromDBByDoc(personDoc);
                        if (GetAge(avvDocument.BirthDate.Value) < 16)
                        {
                            return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType>() { ErrorType.BirthDate } };
                        }
                        ///////////4.2.9.2.1
                        if (ReplaceEV(PersonInfo.FirstName.Value).ToUpper() == ReplaceEV(avvDocument.FirstName).ToUpper() && ReplaceEV(PersonInfo.LastName.Value).ToUpper() == ReplaceEV(avvDocument.LastName).ToUpper())
                        {
                            return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType> { ErrorType.Residency } };
                        }
                        ///////////4.2.9.2.2
                        else if (ReplaceEV(PersonInfo.FirstName.Value).ToUpper() != ReplaceEV(avvDocument.FirstName).ToUpper() && ReplaceEV(PersonInfo.LastName.Value).ToUpper() != ReplaceEV(avvDocument.LastName).ToUpper())
                        {
                            return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType> { ErrorType.Residency, ErrorType.FirstName, ErrorType.LastName } };
                        }
                        else if (ReplaceEV(PersonInfo.FirstName.Value).ToUpper() != ReplaceEV(avvDocument.FirstName).ToUpper())
                        {
                            return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType> { ErrorType.Residency, ErrorType.FirstName } };
                        }
                        else if (ReplaceEV(PersonInfo.LastName.Value).ToUpper() != ReplaceEV(avvDocument.LastName).ToUpper())
                        {
                            return new ConverterRespModel() { Status = 1, ErrorTypes = new List<ErrorType> { ErrorType.Residency, ErrorType.LastName } };
                        }
                    }
                }
            }
            AddToDicsIfNotExist(PersonInfo.FirstName.Value, PersonInfo.LastName.Value);
            return new ConverterRespModel() { Status = 0 };
        }

        public async Task<NonResidentRespModel>GetPersonInfoBySSN(string ssn)
        {
            List<AcraData.Models.Acra4.BPR_Persons> validDocs = new List<AcraData.Models.Acra4.BPR_Persons>();
            AcraData.Models.Acra4.BPR_Persons aVVPerson = new BPR_Persons();
            if (ssn != null)
            {

                Get3rdSourceInfoBySSN(ssn);
                if (isEkengSuccess == false)
                {
                    return new NonResidentRespModel() { EkengStatus = false };
                }
                else if (responseModel == null || responseModel.Result.Count == 0)
                {
                    return new NonResidentRespModel() { EkengStatus = true, IsValid = false };
                }
                if (responseModel != null && responseModel.Result != null && responseModel.Result.Count == 1)
                {
                    var persons = _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                    aVVPerson = Get3rdSourceInfoFromDBBySSN(ssn);
                    ssn = aVVPerson.PNum;
                }
            }

            validDocs = GetValidDocumentsToJson(ssn);
            if (validDocs.Count == 0)
            {
                return new NonResidentRespModel() { EkengStatus = true, IsValid = true, SSN = aVVPerson.PNum, Persons = validDocs };
            }

            return new NonResidentRespModel() { EkengStatus = true, IsValid = true, SSN = aVVPerson.PNum, Persons = validDocs };

        }
        public async Task<NonResidentRespModel> Validate(string document, string ssn="")
        {
            List<AcraData.Models.Acra4.BPR_Persons> validDocs = new List<AcraData.Models.Acra4.BPR_Persons>();
            AcraData.Models.Acra4.BPR_Persons aVVPerson = new BPR_Persons();
            if (document != null)
            { 
                Get3rdSourceInfoByDoc(document);
                if (isEkengSuccess == false)
                {
                    return new NonResidentRespModel() { EkengStatus = false };
                }
                else if (responseModel == null || responseModel.Result.Count == 0)
                {
                    return new NonResidentRespModel() { EkengStatus = true, IsValid = false };
                }
                if (responseModel != null && responseModel.Result != null && responseModel.Result.Count == 1)
                {
                    var persons = _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                    aVVPerson = Get3rdSourceInfoFromDBByDoc(document).BPR_Persons;
                    ssn = aVVPerson.PNum;
                } 
            }
         
            validDocs =  GetValidDocumentsToJson(ssn);
            if(validDocs.Count==0)
            {
                return new NonResidentRespModel() { EkengStatus = true, IsValid = true, SSN = aVVPerson.PNum, Persons = validDocs };
            }

            return   new NonResidentRespModel() { EkengStatus = true, IsValid = true, SSN = aVVPerson.PNum, Persons = validDocs };
        }
        public NonResidentRespModel ValidateWithoutResidency(PersonWithoutResidency personWR)
        { 
            AcraData.Models.Acra4.BPR_Persons aVVPerson = new BPR_Persons();
            if (personWR.ssn == null)
            {
                if (DocExistanceIn3rdSource(personWR.passport))
                {
                    var aVVDoc = Get3rdSourceInfoFromDBByDoc(personWR.passport);
                    aVVPerson = aVVDoc.BPR_Persons;
                    personWR.ssn = aVVPerson.PNum;

                }
                else
                {
                    Get3rdSourceInfoByDoc(personWR.passport);
                    if (isEkengSuccess == false)
                    {
                        return new NonResidentRespModel() { EkengStatus = false };
                    }
                    else if (responseModel == null || responseModel.Result.Count == 0)
                    {
                        return new NonResidentRespModel() { EkengStatus = true, IsValid = false };
                    }
                    if (responseModel != null && responseModel.Result != null && responseModel.Result.Count == 1)
                    {
                        var persons = _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                        aVVPerson = Get3rdSourceInfoFromDBByDoc(personWR.passport).BPR_Persons;
                        personWR.ssn = aVVPerson.PNum;
                    }
                }

            }
            if (personWR.ssn != null)
            {
                bool isDocExcist = false;
                if (DocExistanceIn3rdSource(personWR.passport))
                {
                    var aVVDoc = Get3rdSourceInfoFromDBByDoc(personWR.passport);
                    aVVPerson = aVVDoc.BPR_Persons;
                    isDocExcist = true; 
                }
                else
                {
                    Get3rdSourceInfoByDoc(personWR.passport);
                    if (isEkengSuccess == false)
                    {
                        return new NonResidentRespModel() { EkengStatus = false };
                    }
                    else if (responseModel == null || responseModel.Result.Count == 0)
                    {
                        Get3rdSourceInfoBySSN(personWR.ssn);
                        if (isEkengSuccess == false)
                        {
                            return new NonResidentRespModel() { EkengStatus = false };
                        }
                        else if (responseModel == null || responseModel.Result.Count == 0)
                        {
                            return new NonResidentRespModel() { EkengStatus = true, IsValid = false, Error = "socialCard" };
                        }
                        else if (responseModel != null && responseModel.Result != null && responseModel.Result.Count > 0)
                        {
                            _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                            aVVPerson = Get3rdSourceInfoFromDBBySSN(personWR.ssn);
                            if (ReplaceEV(aVVPerson.FirstName).ToUpper() == ReplaceEV(personWR.firstName).ToUpper() &&
                                ReplaceEV(aVVPerson.LastName).ToUpper() == ReplaceEV(personWR.lastName).ToUpper())
                            { 
                                return new NonResidentRespModel() { EkengStatus = true, IsValid = false, Error = "passport" };
                            }
                            else
                            {
                                if (ReplaceEV(aVVPerson.FirstName).ToUpper() != ReplaceEV(personWR.firstName).ToUpper() &&
                                ReplaceEV(aVVPerson.LastName).ToUpper() != ReplaceEV(personWR.lastName).ToUpper())
                                {
                                    return new NonResidentRespModel() { EkengStatus = true, IsValid = false, Error = "passport,firstName,lastName" };
                                }
                                if (ReplaceEV(aVVPerson.FirstName).ToUpper() != ReplaceEV(personWR.firstName).ToUpper())
                                {
                                    return new NonResidentRespModel() { EkengStatus = true, IsValid = false, Error = "passport,firstName" };
                                }
                                if (ReplaceEV(aVVPerson.LastName).ToUpper() != ReplaceEV(personWR.lastName).ToUpper())
                                {
                                    return new NonResidentRespModel() { EkengStatus = true, IsValid = false, Error = "passport,lastName" };
                                }
                            }
                        }
                    }
                    if (responseModel != null && responseModel.Result != null && responseModel.Result.Count == 1)
                    {
                        _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                        aVVPerson = Get3rdSourceInfoFromDBByDoc(personWR.passport).BPR_Persons;
                        isDocExcist = true;
                    }
                }
                string errors = string.Empty;
                if (isDocExcist && aVVPerson.PNum != personWR.ssn)
                {
                    BPR_Persons aVVPersonSsn = new BPR_Persons();
                    if (SSNIndicatorExistanceIn3rdSource(personWR.ssn))
                    {
                        aVVPersonSsn = Get3rdSourceInfoFromDBBySsnIndicator(personWR.ssn);
                        if (ReplaceEV(aVVPersonSsn.FirstName).ToUpper() == ReplaceEV(personWR.firstName).ToUpper() &&
                                ReplaceEV(aVVPersonSsn.LastName).ToUpper() == ReplaceEV(personWR.lastName).ToUpper())
                        {
                            return new NonResidentRespModel() { EkengStatus = true, IsValid = false, Error = "passport" };
                        }
                    }
                    else if (SSNExistanceIn3rdSource(personWR.ssn))
                    {
                        aVVPersonSsn = Get3rdSourceInfoFromDBBySSN(personWR.ssn);
                        if (ReplaceEV(aVVPersonSsn.FirstName).ToUpper() == ReplaceEV(personWR.firstName).ToUpper() &&
                                ReplaceEV(aVVPersonSsn.LastName).ToUpper() == ReplaceEV(personWR.lastName).ToUpper())
                        {
                            return new NonResidentRespModel() { EkengStatus = true, IsValid = false, Error = "passport" };
                        }
                    }
                    else
                    {
                        Get3rdSourceInfoBySSN(personWR.ssn);
                        if (isEkengSuccess == false)
                        {
                            return new NonResidentRespModel() { EkengStatus = false };
                        }
                        else if (responseModel != null && responseModel.Result != null && responseModel.Result.Count > 0)
                        {
                            _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                            aVVPersonSsn = Get3rdSourceInfoFromDBBySSN(personWR.ssn);
                            if (ReplaceEV(aVVPersonSsn.FirstName).ToUpper() == ReplaceEV(personWR.firstName).ToUpper() &&
                                ReplaceEV(aVVPersonSsn.LastName).ToUpper() == ReplaceEV(personWR.lastName).ToUpper())
                            {
                                return new NonResidentRespModel() { EkengStatus = true, IsValid = false, Error = "passport" };
                            }
                        }
                    } 

                    errors += "socialCard";
                }
                if (!isDocExcist)
                {
                    if (SSNIndicatorExistanceIn3rdSource(personWR.ssn))
                    {
                        aVVPerson = Get3rdSourceInfoFromDBBySsnIndicator(personWR.ssn);
                    }
                    else if (SSNExistanceIn3rdSource(personWR.ssn))
                    {
                        aVVPerson = Get3rdSourceInfoFromDBBySSN(personWR.ssn);
                    }
                    else
                    {
                        Get3rdSourceInfoBySSN(personWR.ssn);
                        if (isEkengSuccess == false)
                        {
                            return new NonResidentRespModel() { EkengStatus = false };
                        }
                        else if (responseModel == null || responseModel.Result.Count == 0)
                        {
                            return new NonResidentRespModel() { EkengStatus = true, IsValid = false };
                        }
                        else if (responseModel != null && responseModel.Result != null && responseModel.Result.Count > 0)
                        {
                            _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                            aVVPerson = Get3rdSourceInfoFromDBBySSN(personWR.ssn);
                        }
                    }
                }

                //errors += errors == string.Empty ? "" : ",";
                if (!string.IsNullOrEmpty(personWR.firstName))
                {
                    errors += CompareWithoutResidency(aVVPerson, personWR) != string.Empty && errors != string.Empty ? $",{CompareWithoutResidency(aVVPerson, personWR)}" : CompareWithoutResidency(aVVPerson, personWR);
                    if (errors != string.Empty)
                    {
                        return new NonResidentRespModel() { EkengStatus = true, IsValid = false, Error = errors };
                    }
                }
            } 
            return new NonResidentRespModel() { EkengStatus = true, IsValid = true, SSN = aVVPerson.PNum, Persons= new List<BPR_Persons>() };
        }

        private  List<AcraData.Models.Acra4.BPR_Persons>  GetValidDocumentsToJson(string ssn)
        {
            List<AcraData.Models.Acra4.BPR_Persons> aVVPersonsFiltered = new List<AcraData.Models.Acra4.BPR_Persons>();
            DateTime currentDate = DateTime.Now;
            using (Acra4DbContext context = new Acra4DbContext(_acra4DbOptions))
            {
                var query = System.Linq.Queryable
         .Where(context.BPR_Persons, p => p.PNum == ssn)
         .Include(x => x.BPR_Documents); // Include здесь всё равно из EF Core

                Console.WriteLine($"ID After Select :{query.FirstOrDefault().ID}\r\n\r\n");  // SQL-query
                aVVPersonsFiltered = query.ToList();
                
                Console.WriteLine($"ALL DOCUMENTS COUNT FROM DATABASE : {aVVPersonsFiltered.FirstOrDefault().BPR_Documents.Count} \r\n\r\n");               
            }

            // Теперь фильтруем документы в памяти
            foreach (var person in aVVPersonsFiltered)
            {
               person.BPR_Documents = person.BPR_Documents
                    .Where(d => d.DocumentStatus != null && d.DocumentStatus != "INVALID")
                    .ToList();                
            }
            Console.WriteLine($"Finded records: {aVVPersonsFiltered.FirstOrDefault().BPR_Documents.Count}");

            return aVVPersonsFiltered;            
        }

        public bool SSNExistanceIn3rdSource(string SSN)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using var context = new Acra4DbContext(_acra4DbOptions);

            // Используем Any() вместо Where().Count() > 0
            return context.BPR_Persons.Any(p => p.PNum == SSN || p.CertificateNum == SSN);
        }


        public bool DocExistanceIn3rdSource(string Document)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");
            using var context = new Acra4DbContext(_acra4DbOptions);

            // Any() безопаснее и эффективнее, чем Where().Count() > 0
            return context.BPR_Documents.Any(p => p.DocumentNumber == Document);
        }


        public AcraData.Models.Acra4.BPR_Persons Get3rdSourceInfoFromDB(string Document)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");

            using var context = new Acra4DbContext(_acra4DbOptions);

            // Используем FirstOrDefault с условием напрямую
            var docInfo = context.BPR_Documents
                .FirstOrDefault(p => p.DocumentNumber == Document);

            if (docInfo != null)
                return Get3rdSourceInfoFromDB(docInfo.AVVPersonID);

            return null;
        }

        public AcraData.Models.Acra4.BPR_Persons Get3rdSourceInfoFromDB(long AVVPersonID)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");

            using var context = new Acra4DbContext(_acra4DbOptions);

            // FirstOrDefault с Include
            return context.BPR_Persons
                .Include(p => p.BPR_Documents)
                .FirstOrDefault(p => p.ID == AVVPersonID);
        }


        private void Get3rdSourceInfoBySSN(string SSN)
        {
            isEkengSuccess = true;
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            try
            {
                if (SSN != "")
                {
                    Models.AVV.BySSN personData = new Models.AVV.BySSN { psn = SSN, Addresses = Models.AVV.Addresses.CURRENT };
                    var url = $"{_configuration.EkengServiceURL}/AVV/GetPersonInfoBySSN";

                    var client = new HttpClient();
                    client.Timeout = new TimeSpan(0, 1, 0);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(JsonConvert.SerializeObject(personData), Encoding.UTF8, "application/json") };
                    HttpResponseMessage response = client.SendAsync(request).Result;

                    Log3rdSourceRequests(request, response);
                    if (response.IsSuccessStatusCode)
                    {
                        responseModel = JsonConvert.DeserializeObject<Models.AVV.AvvResponse>(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                        isEkengSuccess = false;
                }

            }
            catch (Exception ex)
            {
                isEkengSuccess = false;
                _logger.Log.ErrorFormat("Get3rdSourceInfoByDoc:{0}", ex.Message);
            }
            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
        }
        /// Zaven
        private void Get3rdSourceInfoBySSN(string SSN, bool isDocNotEqual)
        {
            isEkengSuccess = true;
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            try
            {
                if (SSN != "")
                {
                    Models.AVV.BySSN personData = new Models.AVV.BySSN { psn = SSN, Addresses = Models.AVV.Addresses.CURRENT };
                    var url = $"{_configuration.EkengServiceURL}/AVV/GetPersonInfoBySSN";

                    var client = new HttpClient();
                    client.Timeout = new TimeSpan(0, 1, 0);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(JsonConvert.SerializeObject(personData), Encoding.UTF8, "application/json") };
                    HttpResponseMessage response = client.SendAsync(request).Result;

                    Log3rdSourceRequests(request, response);
                    //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} ssn = {SSN}" + Environment.NewLine);
                    //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} response = {response.IsSuccessStatusCode}" + Environment.NewLine);
                    if (response.IsSuccessStatusCode)
                    {
                        ssnResponseModel = JsonConvert.DeserializeObject<Models.AVV.AvvResponse>(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                        isEkengSuccess = false;
                }
            }
            catch (Exception ex)
            {
                isEkengSuccess = false;
                _logger.Log.ErrorFormat("Get3rdSourceInfoByDoc:{0}", ex.Message);
            }
            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
        }
        private void Get3rdSourceInfoByDoc(string Document)
        {
            isEkengSuccess = true;
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            try
            {
                if (Document != "")
                {
                    Models.AVV.ByDocument personData = new Models.AVV.ByDocument { docnum = Document, Addresses = Models.AVV.Addresses.CURRENT };
                    var url = $"{_configuration.EkengServiceURL}/AVV/GetPersonInfoByDocument";

                    var client = new HttpClient();
                    client.Timeout = new TimeSpan(0, 1, 0);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new StringContent(JsonConvert.SerializeObject(personData), Encoding.UTF8, "application/json") };
                     HttpResponseMessage response =   client.Send(request);
                    Log3rdSourceRequests(request, response);
                    if (response.IsSuccessStatusCode)
                    {
                        responseModel = JsonConvert.DeserializeObject<Models.AVV.AvvResponse>(  response.Content.ReadAsStringAsync().Result);
                    }
                    else
                        isEkengSuccess = false;
                }
            }
            catch (Exception ex)
            {
                isEkengSuccess = false;
                _logger.Log.ErrorFormat("Get3rdSourceInfoByDoc:{0}", ex.Message);
            }
            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
        }

        private void Log3rdSourceRequests(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage)
        {
            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   8" + Environment.NewLine);
            _logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            using (var context = new AcraData.Data.AcraJournalDbContext(_acraJournalOptions))
            {
                try
                {
                     context.BPR_Transaction.AddAsync(new BPR_Transaction() {
                        Request =  requestMessage.Content.ReadAsStringAsync().Result, 
                        Response =   responseMessage.Content.ReadAsStringAsync().Result, 
                        ResponseDateTime = DateTime.Now });

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.Log.Error($"Error Method: {MethodBase.GetCurrentMethod().Name} Error:{ex.Message}");
                }
            }
            _logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
        }

        private AcraData.Models.Acra4.BPR_Persons Get3rdSourceInfoFromDBBySSN(string SSN)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");

            using var context = new Acra4DbContext(_acra4DbOptions);

            // FirstOrDefault с условием напрямую
            return context.BPR_Persons
                .Include(p => p.BPR_Documents)
                .FirstOrDefault(p => p.PNum == SSN);
        }

        public bool SSNIndicatorExistanceIn3rdSource(string ssnIndicator)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");

            using var context = new Acra4DbContext(_acra4DbOptions);

            // Any() безопаснее и эффективнее
            return context.BPR_Persons.Any(p => p.CertificateNum == ssnIndicator);
        }

        private AcraData.Models.Acra4.BPR_Persons Get3rdSourceInfoFromDBBySsnIndicator(string ssnIndicator)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");

            using var context = new Acra4DbContext(_acra4DbOptions);

            // FirstOrDefault с условием напрямую + Include
            return context.BPR_Persons
                .Include(p => p.BPR_Documents)
                .FirstOrDefault(p => p.CertificateNum == ssnIndicator);
        }

        private AcraData.Models.Acra4.BPR_Documents Get3rdSourceInfoFromDBByDoc(string Document)
        {
            _logger.Log.Info($"Start Ret Method: {MethodBase.GetCurrentMethod().Name}");

            using var context = new Acra4DbContext(_acra4DbOptions);

            // FirstOrDefault с условием напрямую
            var aVVDocument = context.BPR_Documents
                .FirstOrDefault(p => p.DocumentNumber == Document);

            if (aVVDocument != null)
            {
                // Подтягиваем связанного человека
                aVVDocument.BPR_Persons = context.BPR_Persons
                    .FirstOrDefault(x => x.ID == aVVDocument.AVVPersonID);
            }

            return aVVDocument;
        }

        private string ReplaceEV(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                if (str.IndexOf('և') >= 0)
                {
                    str = str.Replace("և", "ԵՎ");
                }
            }
           
            return str;
        }

        private int GetAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }
        private string CompareWithoutResidency(BPR_Persons aVVPerson, PersonWithoutResidency personWR)
        {
            string errors = string.Empty;

            // Сравнение FirstName
            if ((ReplaceEV(aVVPerson.FirstName)?.ToUpper() ?? "") != (ReplaceEV(personWR.firstName)?.ToUpper() ?? ""))
                errors += errors == string.Empty ? "firstName" : ",firstName";

            // Сравнение LastName
            if ((ReplaceEV(aVVPerson.LastName)?.ToUpper() ?? "") != (ReplaceEV(personWR.lastName)?.ToUpper() ?? ""))
                errors += errors == string.Empty ? "lastName" : ",lastName";

            List<string> docNums;
            using (var context = new Acra4DbContext(_acra4DbOptions))
            {
                // Явный Queryable, чтобы убрать ambiguous Where
                docNums = System.Linq.Queryable
                    .Where(context.BPR_Documents, x => x.AVVPersonID == aVVPerson.ID)
                    .Select(x => x.DocumentNumber)
                    .ToList();
            }

            // Проверка паспорта
            if (!docNums.Contains(personWR.passport))
                errors += errors == string.Empty ? "passport" : ",passport";

            // Проверка idCard
            if (!string.IsNullOrEmpty(personWR.idCard) && !docNums.Contains(personWR.idCard))
                errors += errors == string.Empty ? "idCard" : ",idCard";

            // Проверка даты рождения
            if (!string.IsNullOrEmpty(personWR.birthDate) &&
                DateTime.TryParseExact(personWR.birthDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate) &&
                aVVPerson.BirthDate != parsedDate)
            {
                errors += errors == string.Empty ? "birthDate" : ",birthDate";
            }

            return errors;
        }

        private void AddToDicsIfNotExist(string firstName, string LastName)
        {
            using (var context = new Acra3DbContext(_acra3DbOptions))
            {

                //     var tmpFirstName = Encodings.GetCP1252string(firstName);
                //     var tmpLastName = Encodings.GetCP1252string(LastName);

                string queryFirstName = $"SELECT FirstName FROM DicFirstNames WHERE FirstName = CONVERT(CAST(CONVERT('{ReplaceEV(firstName).ToUpper()}' USING UTF8) AS BINARY) USING LATIN1)";
                var resultFirstName = context.RawSqlQuery<string>(queryFirstName, p => p["FirstName"].ToString());

                /* List<string> firstNames = (List<string>)context.DicFirstNames.Select(x => x.FirstName.ToString()).ToList();

                 foreach (var qfirstName in firstNames) {
                     if (qfirstName == firstName)
                     {
                         flag = false;
                         break;
                     }
                 }
                */
                if (resultFirstName.Count == 0)
                {
                    context.Database.ExecuteSqlRaw($"INSERT INTO DicFirstNames (FirstName) VALUES (CONVERT(CAST(CONVERT('{ReplaceEV(firstName).ToUpper()}' USING UTF8) AS BINARY) USING LATIN1))");
                }

                /*    List<string> lastNames = (List<string>)context.DicLastNames.Select(x => x.LastName.ToString()).ToList();
                    flag = true;
                    foreach (var qlastName in lastNames) {
                        if (qlastName == LastName) {
                                flag = false;
                                break;   
                        }

                    } */

                string queryLastName = $"SELECT LastName FROM DicLastNames WHERE LastName = CONVERT(CAST(CONVERT('{ReplaceEV(LastName).ToUpper()}' USING UTF8) AS BINARY) USING LATIN1)";
                var resultLastName = context.RawSqlQuery<string>(queryLastName, p => p["LastName"].ToString());

                if (resultLastName.Count == 0)
                {
                    context.Database.ExecuteSqlRaw($"INSERT INTO DicLastNames (LastName) VALUES (CONVERT(CAST(CONVERT('{ReplaceEV(LastName).ToUpper()}'USING UTF8) AS BINARY) USING LATIN1))");
                }

            }
        }

        private string getISO(string message)
        {

            Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            Encoding utf8 = Encoding.UTF8;
            byte[] utfBytes = utf8.GetBytes(message);
            byte[] isoBytes = Encoding.Convert(utf8, iso, utfBytes);
            return iso.GetString(isoBytes);
        }
    }
}
