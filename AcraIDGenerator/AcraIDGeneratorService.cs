using AcraData.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AcraData.Models.Acra3;
using AcraData.Models.Acra4;
using AcraUtils;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using AcraIDServices.Mappers;
using System.Net.Mail;

namespace AcraIDGenerator
{
    public class AcraIDGeneratorService
    {
        private DbContextOptions<Acra3DbContext> _acra3DbContextOptions;
        private DbContextOptions<Acra4DbContext> _acra4DbContextOptions;
        private DbContextOptions<AcraJournalDbContext> _acraJournalDbContextOptions;
        private AcraUtils.Configuration.AcraIDGeneratorConfig _configuration;

        private AcraIDServices.Models.AVV.AvvResponse responseModel = new AcraIDServices.Models.AVV.AvvResponse();
        private bool isEkengSuccess = true;

        AVVMapper _avvMapper;
        private Logger _logger;

        [ThreadStatic]
        public static BPR_Persons aVVPerson;
        [ThreadStatic]
        public static Person globalPerson;
        public static bool isOneTime;
        public static List<Task> tasks = new List<Task>();
        public static bool isMailSent = false;
        private static Mutex mutex;//= new Mutex();
        public static int testCounter = 0;
        int checkCount;

        public AcraIDGeneratorService(DbContextOptions<Acra3DbContext> acra3Options, DbContextOptions<Acra4DbContext> acra4Options, DbContextOptions<AcraJournalDbContext> acraJournalOptions,
             IOptions<AcraUtils.Configuration.AcraIDGeneratorConfig> configuration, Logger logger)
        {
            _acra3DbContextOptions = acra3Options;
            _acra4DbContextOptions = acra4Options;
            _configuration = configuration.Value;
            _logger = logger;
            _avvMapper = new AVVMapper(_logger, acra4Options);
            _acraJournalDbContextOptions = acraJournalOptions;
        }
        public void start()
        {
            var process = new Task(AcraIdGenerate);
            process.Start();
        }

        public void AcraIdGenerate()
        {
            isOneTime = false;
            Console.WriteLine("isOneTime");
            tasks = new List<Task>();
            Console.WriteLine("tasks");

            while (true)
            {
                System.Threading.Thread.Sleep(60000);
                Console.WriteLine("System.Threading.Thread.Sleep(60000)");

                using (var context = new Acra3DbContext(_acra3DbContextOptions))
                {
                    Console.WriteLine("var context = new Acra3DbContext(_acra3DbContextOptions)");
                    int page = 1;
                    while (context.TriggerActivityTmps.Any(s => s.ActivityType == 6 && s.Status == 0))
                    {
                        Console.WriteLine("context.TriggerActivityTmps.Any(s => s.ActivityType == 6 && s.Status == 0)");
                        int pageSize = 10;
                        if (Queryable.Where(context.TriggerActivityTmps, s => s.ActivityType == 6).Where(s => s.Status == 0).Count() < (page * pageSize))
                        {
                            Console.WriteLine("context.TriggerActivityTmps.Where(s => s.ActivityType == 6).Where(s => s.Status == 0).Count() < (page * pageSize)");
                            page = 1;
                        }
                        var newPersons = Queryable.Where(context.TriggerActivityTmps, s => s.ActivityType == 6).Where(s => s.Status == 0).OrderBy(t => t.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();
                        Console.WriteLine($"newPersons.GetType() {newPersons.GetType()} newPersons.Count {newPersons.Count()}");
                        Console.WriteLine("var newPersons = context.TriggerActivityTmps.Where(s => s.ActivityType == 6).Where(s => s.Status == 0).OrderBy(t => t.Id).Skip((page - 1) * pageSize).Take(pageSize).ToList();");
                        page++;
                        TriggerActivityTemp errItem = new TriggerActivityTemp();
                        try
                        {
                            if (newPersons.Count() > 0)
                            {
                                int taskCounter = 0;
                                foreach (var item in newPersons)
                                {
                                    errItem = item;
                                    var tempPerson = Queryable.Where(context.Persons, x => x.PersonId == Convert.ToUInt32(item.ActivityId)).FirstOrDefault();
                                 //   Console.WriteLine($"tempPerson.GetType() {tempPerson.GetType()}");
                                    Console.WriteLine("var tempPerson = context.Persons.Where(x => x.PersonId == Convert.ToUInt32(item.ActivityId)).FirstOrDefault();");
                                    if(tempPerson == null)
                                    {
                                        continue;
                                    }

                                    if (Queryable.Where(context.Sources, x => x.SourceId == tempPerson.SourceId).FirstOrDefault().GenerateAcraID.ToUpper() == "YES")
                                    {
                                        Console.WriteLine("context.Sources.Where(x => x.SourceId == tempPerson.SourceId).FirstOrDefault().GenerateAcraID.ToUpper() =='YES'");
                                        Task task = new Task(() => CheckByThreads(tempPerson, item.Id));
                                        task.Start();
                                        tasks.Add(task);
                                        taskCounter++;
                                        if (taskCounter == _configuration.ThreadCount)
                                        {
                                            Task.WaitAll(tasks.ToArray());
                                            tasks.RemoveAll(x => x.IsCompleted);
                                            taskCounter = 0;
                                        }
                                    }
                                }
                            }
                            callGC();
                            //TODO: in Real change sleep time to 60000
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{DateTime.Now} Message: {ex.Message} Inner Exception {ex.InnerException} Trace: {ex.StackTrace} ");
                            _logger.Log.Debug($"{DateTime.Now} Message: {ex.Message} Trace: {ex.StackTrace} ");
                            SendEmail($"ACRAID stoped working: Message: {ex.Message} Trace: {ex.StackTrace} ");
                            callGC();
                            break;
                        }
                    }

                }
            }
            
        }

        public void  callGC() {
            GC.Collect(GC.MaxGeneration);
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }


        public void OneTimeStart()
        {
            try
            {
                tasks = new List<Task> { };
                isOneTime = true;
                using (var context = new Acra3DbContext(_acra3DbContextOptions))
                {

                    //var test = context.AcraID_Errors.ToList();

                    //List<string> passports = GetPassportsAndIDCard(937474);
                    int checkSucess = 1;
                    /*
                     * checkstatus 
                     * 0 is OK
                     * 1 is Try again
                     * 2 is Sent to cleansing
                     */
                   /* var generatableSources = context.Sources.Where(x => x.GenerateAcraID == "Yes").ToList();
                    List<uint> sourceIds = new List<uint>();
                    foreach (var item in generatableSources)
                    {
                        sourceIds.Add(Convert.ToUInt32(item.SourceId));
                    }*/
                    context.Database.SetCommandTimeout(36000000);
                    /*     List<uint> exludedPersonsId = new List<uint>() { 11000525, 9385828, 9861248, 13422794, 13605507 };
                         List<Person> persons = context.Persons.Where(x => sourceIds.Contains(x.SourceId.Value) && !exludedPersonsId.Contains(x.PersonId)
                         && (context.ACRAID_MAPPER.Where(y => y.PersonID == x.PersonId).FirstOrDefault() == null) 
                         && (context.AcraID_Errors.Where(z => z.PersonId == x.PersonId).FirstOrDefault() == null)).ToList();*/
                    List<Person> persons = new List<Person>();
                    string query = "SELECT `p`.`PersonID`, `p`.`BirthDate`, `p`.`FirstName`, `p`.`IncomingDate`, `p`.`LastName`, `p`.`PatronymicName`,  `p`.`ResidentID`, `p`.`Sex`, `p`.`SocialCard`, `p`.`SourceID` FROM `Persons` AS `p` LEFT JOIN `Sources` AS `s` ON(`s`.`SourceID` = `p`.`SourceID`) WHERE `s`.`GenerateAcraID` = 'Yes' AND( ( `p`.`PersonID` NOT IN( 11000525, 9385828, 9861248, 13422794, 13605507, 15697738, 15717377 ) ) AND `p`.`PersonID` NOT IN (SELECT `PersonID` FROM `ACRAID_MAPPER`) ) AND  `p`.`PersonID` NOT IN (SELECT `PersonId` FROM `AcraID_Errors` AS `a0`)";
                    persons = context.RawSqlQuery<Person>(query.ToString(), p => new Person
                    {
                        PersonId = Convert.ToUInt32(p["PersonID"]),
                        BirthDate = (DateTime?) (p["BirthDate"].ToString().Length != 0 ? p["BirthDate"] : null),
                        FirstName = (uint?)(p["FirstName"].ToString().Length != 0 ? p["FirstName"] : null),
                        LastName = (uint?)(p["LastName"].ToString().Length != 0 ? p["LastName"] : null),
                        IncomingDate = Convert.ToDateTime(p["IncomingDate"]),
                        PatronymicName = p["PatronymicName"].ToString(),
                        ResidentId = (Byte?)(p["ResidentID"].ToString().Length != 0 ? p["ResidentID"] : null),
                        Sex = (uint?)( p["Sex"].ToString().Length != 0 ? p["Sex"] : null),
                        SocialCard = p["SocialCard"].ToString(),
                        SourceId = (uint?)( p["SourceID"].ToString().Length != 0 ? p["SourceID"] : null)
                    });
                    _logger.Log.Debug($"{DateTime.Now} Started 1000");
                    int taskCounter = 0;
                    foreach (var item in persons)
                    {
                        testCounter++;
                        Console.WriteLine($"{testCounter} of {persons.Count}\n");
                        Task task = new Task(() => CheckByThreads(item));
                        task.Start();
                        tasks.Add(task);
                        taskCounter++;
                        if (taskCounter == _configuration.ThreadCount)
                        {
                            Task.WaitAll(tasks.ToArray());
                            taskCounter = 0;
                            tasks = new List<Task>();
                        }

                        //debugCounter++;
                        //checkSucess = 1;
                        //while (checkSucess != 0)
                        //{
                        //    checkSucess = CheckPerson(item);
                        //    if (checkSucess == 2)
                        //    {
                        //        using (var tx = context.Database.BeginTransaction())
                        //        {
                        //            TriggerActivityTemp entity = new TriggerActivityTemp();
                        //            entity.ActivityId = item.PersonId;
                        //            entity.ActivityType = 6;
                        //            entity.Status = 500;
                        //            context.TriggerActivityTmps.Add(entity);
                        //            context.SaveChanges();
                        //            tx.Commit();
                        //        }
                        //        break;
                        //    }
                        //}
                    }
                    if (tasks.Count > 0)
                    {
                        Task.WaitAll(tasks.ToArray());
                    }
                    _logger.Log.Debug($"{DateTime.Now} Ended 1000");
                    //while (checkSucess != 0)
                    //{
                    //    checkSucess = CheckPerson(context.Persons.Where(x => x.PersonId == 456522360).FirstOrDefault());
                    //    if (checkSucess == 2)
                    //    {
                    //        using (var tx = context.Database.BeginTransaction())
                    //        {
                    //            TriggerActivityTemp entity = new TriggerActivityTemp();
                    //            entity.ActivityId = 456522360;
                    //            entity.ActivityType = 4;
                    //            entity.Status = 500;
                    //            context.TriggerActivityTmps.Add(entity);
                    //            context.SaveChanges();
                    //            tx.Commit();
                    //        }
                    //        break;
                    //    }
                    //}
                    /////// if personIdn ka, uremn chanel

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.Log.Error($"{ex}");
                throw;
            }

        }
        private void CheckByThreads(Person person, long tempTriggerActivityId = 0)
        {
            try {
                using (var context = new Acra3DbContext(_acra3DbContextOptions))
                {
                    if (context.ACRAID_MAPPER.FirstOrDefault(x => x.PersonID == person.PersonId) != null)
                    {
                        try
                        {
                            using (var tx = context.Database.BeginTransaction())
                            {
                                Queryable.Where(context.TriggerActivityTmps, x => x.ActivityId.ToString() == person.PersonId.ToString() && x.ActivityType == 6).FirstOrDefault().Status = 500;
                                context.SaveChanges();
                                tx.Commit();

                            }
                        }catch(Exception ex)
                        {
                            Console.WriteLine("Inner Exception: " + ex.InnerException.Message);
                        }
                        
                        return;
                    }
                    int checkSucess = 1;
                    checkCount = 0;
                    while (checkSucess != 0)
                    {
                        globalPerson = new Person();
                        if (checkCount == 2) //If more than twice means couldnt import person
                        {
                            using (var tx = context.Database.BeginTransaction())
                            {
                                AcraID_Errors error = new AcraID_Errors();
                                error.PersonId = person.PersonId;
                                error.Field = "Ekeng";
                                error.Value1 = "Error importing Person";
                                error.Value2 = "";
                                error.Isavv = 1;
                                error.Status = 0;
                                error.Date = DateTime.Now;
                                context.AcraID_Errors.Add(error);
                                context.SaveChanges();
                                tx.Commit();
                            }
                            break;
                        }
                        checkCount++;
                        //tokenSource = new CancellationTokenSource();
                        checkSucess = CheckPerson(person);
                        //if (checkSucess == 3)
                        //{
                        //    break;
                        //}
                        if (checkSucess == 0 && !isOneTime)
                        {
                            using (var tx = context.Database.BeginTransaction())
                            {
                                Queryable.Where(context.TriggerActivityTmps, x => x.Id == tempTriggerActivityId).FirstOrDefault().Status = 500;
                                context.SaveChanges();
                                tx.Commit();

                            }
                        }
                        if (checkSucess == 2)
                        {
                            if (isOneTime)
                            {
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    TriggerActivityTemp entity = new TriggerActivityTemp();
                                    entity.ActivityId = person.PersonId;
                                    entity.ActivityType = 6;
                                    entity.Status = 200;
                                    context.TriggerActivityTmps.Add(entity);
                                    context.SaveChanges();
                                    tx.Commit();
                                }
                            }
                            else
                            {
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    Queryable.Where(context.TriggerActivityTmps, x => x.Id == tempTriggerActivityId).FirstOrDefault().Status = 200;
                                    context.SaveChanges();
                                    tx.Commit();

                                }
                            }

                            if (aVVPerson != null)
                            {
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "SSN";
                                    error.Value1 = person.SocialCard;
                                    error.Value2 = aVVPerson.PNum;
                                    error.Isavv = 1;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "FirstName";
                                    error.Value1 = GetFirstName(person.FirstName) != null ? Encodings.GetCP1252string(GetFirstName(person.FirstName)) : null;
                                    error.Value2 = aVVPerson.FirstName != null ? Encodings.GetCP1252string(aVVPerson.FirstName) : null;
                                    error.Isavv = 1;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "LastName";
                                    error.Value1 = GetLastName(person.LastName) != null ? Encodings.GetCP1252string(GetLastName(person.LastName)) : null;
                                    error.Value2 = aVVPerson.LastName != null ? Encodings.GetCP1252string(aVVPerson.LastName) : null;
                                    error.Isavv = 1;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "PersonId";
                                    error.Value1 = person.PersonId.ToString();
                                    error.Value2 = aVVPerson.ID.ToString();
                                    error.Isavv = 1;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }

                                List<string> tempAvvDocs = GetAVVDocuments(aVVPerson.ID);
                                List<string> tempDocs = GetPassportsAndIDCard(person.PersonId);

                                if (tempAvvDocs.Count > tempDocs.Count)
                                {
                                    for (int i = 0; i < tempAvvDocs.Count; i++)
                                    {
                                        using (var tx = context.Database.BeginTransaction())
                                        {
                                            AcraID_Errors error = new AcraID_Errors();
                                            if (i < tempDocs.Count)
                                            {
                                                error.PersonId = person.PersonId;
                                                error.Field = "Documnent";
                                                error.Value1 = tempDocs[i];
                                                error.Value2 = tempAvvDocs[i];
                                                error.Isavv = 1;
                                                error.Status = 0;
                                                error.Date = DateTime.Now;
                                                context.AcraID_Errors.Add(error);
                                                context.SaveChanges();
                                                tx.Commit();
                                            }
                                            else
                                            {
                                                error.PersonId = person.PersonId;
                                                error.Field = "Documnent";
                                                error.Value1 = "";
                                                error.Value2 = tempAvvDocs[i];
                                                error.Isavv = 1;
                                                error.Status = 0;
                                                error.Date = DateTime.Now;
                                                context.AcraID_Errors.Add(error);
                                                context.SaveChanges();
                                                tx.Commit();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < tempDocs.Count; i++)
                                    {
                                        using (var tx = context.Database.BeginTransaction())
                                        {
                                            AcraID_Errors error = new AcraID_Errors();
                                            if (i < tempAvvDocs.Count)
                                            {
                                                error.PersonId = person.PersonId;
                                                error.Field = "Documnent";
                                                error.Value1 = tempDocs[i];
                                                error.Value2 = tempAvvDocs[i];
                                                error.Isavv = 1;
                                                error.Status = 0;
                                                error.Date = DateTime.Now;
                                                context.AcraID_Errors.Add(error);
                                                context.SaveChanges();
                                                tx.Commit();
                                            }
                                            else
                                            {
                                                error.PersonId = person.PersonId;
                                                error.Field = "Documnent";
                                                error.Value1 = tempDocs[i];
                                                error.Value2 = "";
                                                error.Isavv = 1;
                                                error.Status = 0;
                                                error.Date = DateTime.Now;
                                                context.AcraID_Errors.Add(error);
                                                context.SaveChanges();
                                                tx.Commit();
                                            }
                                        }
                                    }
                                }

                            }
                            else if (globalPerson != null && globalPerson.PersonId != 0)
                            {
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "SSN";
                                    error.Value1 = person.SocialCard;
                                    error.Value2 = globalPerson.SocialCard;
                                    error.Isavv = 0;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "FirstName";
                                    error.Value1 = GetFirstName(person.FirstName) != null ? Encodings.GetCP1252string(GetFirstName(person.FirstName)) : null;
                                    error.Value2 = GetFirstName(globalPerson.FirstName) != null ? Encodings.GetCP1252string(GetFirstName(globalPerson.FirstName)) : null;
                                    error.Isavv = 0;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "LastName";
                                    error.Value1 = GetLastName(person.LastName) != null ? Encodings.GetCP1252string(GetLastName(person.LastName)) : null;
                                    error.Value2 = GetLastName(globalPerson.LastName) != null ? Encodings.GetCP1252string(GetLastName(globalPerson.LastName)) : null;
                                    error.Isavv = 0;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "PersonId";
                                    error.Value1 = person.PersonId.ToString();
                                    error.Value2 = globalPerson.PersonId.ToString();
                                    error.Isavv = 0;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }

                                List<string> tempGlobalDocs = GetPassportsAndIDCard(globalPerson.PersonId);
                                List<string> tempDocs = GetPassportsAndIDCard(person.PersonId);

                                if (tempGlobalDocs.Count > tempDocs.Count)
                                {
                                    for (int i = 0; i < tempGlobalDocs.Count; i++)
                                    {
                                        using (var tx = context.Database.BeginTransaction())
                                        {
                                            AcraID_Errors error = new AcraID_Errors();
                                            if (i < tempDocs.Count)
                                            {
                                                error.PersonId = person.PersonId;
                                                error.Field = "Documnent";
                                                error.Value1 = tempDocs[i];
                                                error.Value2 = tempGlobalDocs[i];
                                                error.Isavv = 0;
                                                error.Status = 0;
                                                error.Date = DateTime.Now;
                                                context.AcraID_Errors.Add(error);
                                                context.SaveChanges();
                                                tx.Commit();
                                            }
                                            else
                                            {
                                                error.PersonId = person.PersonId;
                                                error.Field = "Documnent";
                                                error.Value1 = "";
                                                error.Value2 = tempGlobalDocs[i];
                                                error.Isavv = 0;
                                                error.Status = 0;
                                                error.Date = DateTime.Now;
                                                context.AcraID_Errors.Add(error);
                                                context.SaveChanges();
                                                tx.Commit();
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < tempDocs.Count; i++)
                                    {
                                        using (var tx = context.Database.BeginTransaction())
                                        {
                                            AcraID_Errors error = new AcraID_Errors();
                                            if (i < tempGlobalDocs.Count)
                                            {
                                                error.PersonId = person.PersonId;
                                                error.Field = "Documnent";
                                                error.Value1 = tempDocs[i];
                                                error.Value2 = tempGlobalDocs[i];
                                                error.Isavv = 0;
                                                error.Status = 0;
                                                error.Date = DateTime.Now;
                                                context.AcraID_Errors.Add(error);
                                                context.SaveChanges();
                                                tx.Commit();
                                            }
                                            else
                                            {
                                                error.PersonId = person.PersonId;
                                                error.Field = "Documnent";
                                                error.Value1 = tempDocs[i];
                                                error.Value2 = "";
                                                error.Isavv = 0;
                                                error.Status = 0;
                                                error.Date = DateTime.Now;
                                                context.AcraID_Errors.Add(error);
                                                context.SaveChanges();
                                                tx.Commit();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "SSN";
                                    error.Value1 = person.SocialCard;
                                    error.Value2 = "";
                                    error.Isavv = 0;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "FirstName";
                                    error.Value1 = GetFirstName(person.FirstName) != null ? Encodings.GetCP1252string(GetFirstName(person.FirstName)) : null;
                                    error.Value2 = "";
                                    error.Isavv = 0;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "LastName";
                                    error.Value1 = GetLastName(person.LastName) != null ? Encodings.GetCP1252string(GetLastName(person.LastName)) : null;
                                    error.Value2 = "";
                                    error.Isavv = 0;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }
                                using (var tx = context.Database.BeginTransaction())
                                {
                                    AcraID_Errors error = new AcraID_Errors();
                                    error.PersonId = person.PersonId;
                                    error.Field = "PersonId";
                                    error.Value1 = person.PersonId.ToString();
                                    error.Value2 = "";
                                    error.Isavv = 0;
                                    error.Status = 0;
                                    error.Date = DateTime.Now;
                                    context.AcraID_Errors.Add(error);
                                    context.SaveChanges();
                                    tx.Commit();
                                }

                                List<string> tempDocs = GetPassportsAndIDCard(person.PersonId);
                                foreach (var item in tempDocs)
                                {
                                    using (var tx = context.Database.BeginTransaction())
                                    {
                                        AcraID_Errors error = new AcraID_Errors();
                                        error.PersonId = person.PersonId;
                                        error.Field = "Documnent";
                                        error.Value1 = item;
                                        error.Value2 = "";
                                        error.Isavv = 0;
                                        error.Status = 0;
                                        error.Date = DateTime.Now;
                                        context.AcraID_Errors.Add(error);
                                        context.SaveChanges();
                                        tx.Commit();
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.Log.Error($"{ex}");
                SendEmail("ACRAID System erorr: " + ex.ToString());
            }
        }
        public int CheckPerson(Person person)
        {
            //if (testCounter != 6751)
            //{
            //    return 3;
            //}
            //if (testCounter == 6751 || testCounter == 7193 || testCounter == 22319 || testCounter == 28461)
            //{
            //    return 3;
            //}
            ///////4.2.12
            if (person.SocialCard != null && Regex.IsMatch(person.SocialCard, "^[0-9]{10}$"))
            {
                aVVPerson = GetAVVPerson(person.SocialCard);
                if (aVVPerson != null)
                {
                    int matchingId = Compare(aVVPerson, person);
                    if (matchingId != 0)
                    {
                        if (Regex.IsMatch(aVVPerson.PNum, "^[0-9]{10}$"))
                            GenerateAcraID(long.Parse(aVVPerson.PNum), person.PersonId, matchingId);
                        else
                            GenerateAcraID(GenerateAcraIDForCertificateNum(), person.PersonId, matchingId);
                        return 0;
                    }
                    else ///////////4.2.12.2
                    {
                        if (aVVPerson.AVVGetDate.Value.Date != DateTime.Today)
                        {
                            Get3rdSourceInfoBySSN(aVVPerson.PNum);
                            if (!isEkengSuccess || responseModel == null )
                            {
                                if (isEkengSuccess && (responseModel == null || responseModel.Result == null)) {
                                    return 2;
                                }
                                mutex = new Mutex();
                                mutex.WaitOne();
                                checkCount--;
                                if (!isMailSent)
                                {
                                    SendEmail("Ekeng is unavailable for AcraID Service. Service will try again after 30 minutes");
                                    isMailSent = true;
                                }
                                mutex.ReleaseMutex();
                                _logger.Log.Error($"Ekeng is unavailable");
                                mutex.WaitOne();
                                System.Threading.Thread.Sleep(3000);
                                isMailSent = false;
                                isEkengSuccess = true;

                                mutex.ReleaseMutex();
                                return 1;
                            }
                            else
                            {
                                _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                                return 1;
                            }
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }
                else /////////4.2.12.3
                {
                    Get3rdSourceInfoBySSN(person.SocialCard);
                    if (!isEkengSuccess)
                    {
                        mutex = new Mutex();
                        mutex.WaitOne();
                        if (!isMailSent)
                        {
                            SendEmail("Ekeng is unavailable for AcraID Service. Service will try again after 30 minutes");
                            isMailSent = true;
                        }
                        mutex.ReleaseMutex();
                        _logger.Log.Error($"Ekeng is unavailable");
                        mutex.WaitOne();
                        System.Threading.Thread.Sleep(6000);
                        isMailSent = false;
                        isEkengSuccess = true;
                        mutex.ReleaseMutex();
                        return 1;

                    }
                    if (responseModel == null || responseModel.Result == null)
                    {
                        return 2;
                    }
                    if (responseModel != null && responseModel.Result != null)
                    {
                        _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                        return 1;
                    }
                    return 0;
                }
            }
            /////////////4.2.13
            if (person.SocialCard != null && person.SocialCard != "")
            {
                aVVPerson = GetAVVPerson(person.SocialCard);
                if (aVVPerson != null)
                {
                    int matchingId = Compare(aVVPerson, person);
                    if (matchingId != 0) /////////4.2.13.1
                    {
                        if (Regex.IsMatch(aVVPerson.PNum, "^[0-9]{10}$"))
                            GenerateAcraID(long.Parse(aVVPerson.PNum), person.PersonId, matchingId);
                        else
                            GenerateAcraID(GenerateAcraIDForCertificateNum(), person.PersonId, matchingId);
                        return 0;
                    }
                    else //////////4.2.13.2
                    {
                        if (aVVPerson.AVVGetDate.Value.Date != DateTime.Today)
                        {
                            Get3rdSourceInfoBySSN(aVVPerson.PNum);
                            if (!isEkengSuccess || responseModel == null || responseModel.Result == null)
                            {
                                mutex = new Mutex();
                                mutex.WaitOne();
                                if (!isMailSent)
                                {
                                    SendEmail("Ekeng is unavailable for AcraID Service. Service will try again after 30 minutes");
                                    isMailSent = true;
                                }
                                mutex.ReleaseMutex();
                                _logger.Log.Error($"Ekeng is unavailable");
                                mutex.WaitOne();
                                System.Threading.Thread.Sleep(6000);
                                isMailSent = false;
                                isEkengSuccess = true;
                                mutex.ReleaseMutex();
                                return 1;
                            }
                            else
                            {
                                _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                                return 1;
                            }
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }
                else /////////4.2.13.3
                {
                    Get3rdSourceInfoBySSN(person.SocialCard);
                    if (!isEkengSuccess)
                    {
                        mutex = new Mutex();
                        mutex.WaitOne();
                        if (!isMailSent)
                        {
                            SendEmail("Ekeng is unavailable for AcraID Service. Service will try again after 30 minutes");
                            isMailSent = true;
                        }
                        mutex.ReleaseMutex();
                        _logger.Log.Error($"Ekeng is unavailable");
                        mutex.WaitOne();
                        System.Threading.Thread.Sleep(6000);
                        isMailSent = false;
                        isEkengSuccess = true;
                        mutex.ReleaseMutex();
                        return 1;
                    }
                    if (responseModel == null || responseModel.Result == null)
                    {
                        return 2;
                    }
                    if (responseModel != null && responseModel.Result != null)
                    {
                        _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                        return 1;
                    }
                    return 0;
                }
            }
            if (person.SocialCard == null || person.SocialCard == "") ////////4.2.14
            {
                ///////////4.2.15
                List<string> passports = GetPassportsAndIDCard(person.PersonId);
                if (passports == null || passports.Count < 1)
                {
                    return 2;
                }
                if (GetaVVPersonByDoc(passports) != null)
                {
                    aVVPerson = GetaVVPersonByDoc(passports);
                    int matchingId = Compare(aVVPerson, person);
                    if (matchingId != 0)
                    {
                        if (Regex.IsMatch(aVVPerson.PNum, "^[0-9]{10}$"))
                            GenerateAcraID(long.Parse(aVVPerson.PNum), person.PersonId, matchingId);
                        else
                            GenerateAcraID(GenerateAcraIDForCertificateNum(), person.PersonId, matchingId);
                        return 0;
                    }
                    else
                    {
                        if (aVVPerson.AVVGetDate.Value.Date != DateTime.Today)
                        {
                            Get3rdSourceInfoBySSN(aVVPerson.PNum);
                            if (!isEkengSuccess || responseModel == null || responseModel.Result == null)
                            {
                                mutex = new Mutex();
                                mutex.WaitOne();
                                if (!isMailSent)
                                {
                                    SendEmail("Ekeng is unavailable for AcraID Service. Service will try again after 30 minutes");
                                    isMailSent = true;
                                }
                                mutex.ReleaseMutex();
                                _logger.Log.Error($"Ekeng is unavailable");
                                mutex.WaitOne();
                                System.Threading.Thread.Sleep(6000);
                                isMailSent = false;
                                isEkengSuccess = true;
                                mutex.ReleaseMutex();
                                return 1;
                            }
                            else
                            {
                                _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                                return 1;
                            }
                        }
                        else
                        {
                            return 2;
                        }
                    }
                }
                else
                {
                    foreach (var item in passports)
                    {
                        Get3rdSourceInfoByDoc(item);
                        if (isEkengSuccess != false && responseModel != null && responseModel.Result != null)
                        {
                            break;
                        }
                    }
                    if (!isEkengSuccess)
                    {
                        mutex = new Mutex();
                        mutex.WaitOne();
                        if (!isMailSent)
                        {
                            SendEmail("Ekeng is unavailable for AcraID Service. Service will try again after 30 minutes");
                            isMailSent = true;
                        }
                        mutex.ReleaseMutex();
                        _logger.Log.Error($"Ekeng is unavailable");
                        mutex.WaitOne();
                        System.Threading.Thread.Sleep(6000);
                        isMailSent = false;
                        isEkengSuccess = true;
                        mutex.ReleaseMutex();
                        return 1;
                    }

                    if (responseModel != null && responseModel.Result != null)
                    {
                        _avvMapper.ImportPerson(responseModel.Result.FirstOrDefault());
                        foreach (var item in responseModel.Result.FirstOrDefault().AVVDocuments.Document)
                        {
                            if (item.BasicDocument != null && passports.Contains(item.BasicDocument.Basic_Document_Number))
                            {
                                int matchingId = Compare(responseModel.Result.FirstOrDefault(), person);
                                if (matchingId != 0)
                                {
                                    if (Regex.IsMatch(responseModel.Result.FirstOrDefault().PNum, "^[0-9]{10}$"))
                                        GenerateAcraID(long.Parse(responseModel.Result.FirstOrDefault().PNum), person.PersonId, matchingId);
                                    else
                                        GenerateAcraID(GenerateAcraIDForCertificateNum(), person.PersonId, matchingId);
                                    return 0;
                                }
                                else
                                {
                                    return 2;
                                }
                            }
                        }

                        return 1;
                    }
                    if (responseModel == null || responseModel.Result == null)//////////4.2.17
                    {
                        //////////////4.2.17
                        List<Person> persons = new List<Person>();
                        bool isGenerated = false;
                        foreach (var item in passports)
                        {
                            persons.AddRange(GetAllPersonsByDoc(item, person.PersonId));
                        }
                        persons.AddRange(GetAllPersonsBySameNameAndBirthDate(person));
                        if (persons.Count > 0)
                        {
                            foreach (var item in persons)
                            {
                                globalPerson = item;
                                int matchingId = Compare(person, item);
                                if (matchingId != 0)
                                {
                                    if (PersonIDExistsinAvvMapper(item.PersonId) != null)
                                    {
                                        GenerateAcraID(PersonIDExistsinAvvMapper(item.PersonId).Value, person.PersonId, matchingId);
                                        isGenerated = true;
                                        return 0;
                                    }
                                }
                            }
                            if (!isGenerated)
                            {
                                GenerateAcraID(GenerateAcraIDForNonResident(), person.PersonId, 0);
                                return 0;
                            }

                        }
                        else/////////4.2.17.3
                        {
                            GenerateAcraID(GenerateAcraIDForNonResident(), person.PersonId, 0);
                        }
                    }
                    return 0;
                }

            }
            return 0;
        }
        public void GenerateAcraID(long acraID, uint? personID, int matchingID)
        {
            using (var context = new Acra3DbContext(_acra3DbContextOptions))
            {
                List<uint?> personIds = new List<uint?>();
                foreach (var item in Queryable.Where(context.ACRAID_MAPPER, x => x.ACRAID == acraID).ToList())
                {
                    personIds.Add(item.PersonID);
                }
                if (Queryable.Where(context.ACRAID_MAPPER, x => x.ACRAID == acraID && x.PersonID == personID).FirstOrDefault() == null ||
                    !personIds.Contains(personID))
                {
                    using (var tx = context.Database.BeginTransaction())
                    {
                        ACRAID_MAPPER map = new ACRAID_MAPPER();
                        map.ACRAID = acraID;
                        map.GenerationDate = DateTime.Now;
                        map.MatchingID = (sbyte)matchingID;
                        map.PersonID = personID;
                        context.AddOrUpdate(map);
                        context.SaveChanges();
                        tx.Commit();
                    }
                }
            }
        }
        public long GenerateAcraIDForNonResident()
        {
            using (var context = new Acra3DbContext(_acra3DbContextOptions))
            {
                long? acraID = Queryable.Where(context.ACRAID_MAPPER, x => x.ACRAID.ToString().Length == 12).Max(x => x.ACRAID);
                if (acraID == null)
                {
                    return 100000000001;
                }
                else
                {
                    return acraID.Value + 1;
                }
            }
        }
        public long GenerateAcraIDForCertificateNum()
        {
            using (var context = new Acra3DbContext(_acra3DbContextOptions))
            {
                long? acraID = Queryable.Where(context.ACRAID_MAPPER, x => x.ACRAID.ToString().Length == 13).Max(x => x.ACRAID);
                if (acraID == null)
                {
                    return 1000000000001;
                }
                else
                {
                    return acraID.Value + 1;
                }
            }
        }
        public BPR_Persons GetAVVPerson(string SSN)
        {
            using (var context = new Acra4DbContext(_acra4DbContextOptions))
            {
                if (Queryable.Where(context.BPR_Persons, x => x.PNum == SSN).FirstOrDefault() != null)
                {
                    return Queryable.Where(context.BPR_Persons, x => x.PNum == SSN).FirstOrDefault();
                }
                if (Queryable.Where(context.BPR_Persons, x => x.CertificateNum == SSN).FirstOrDefault() != null)
                {
                    return Queryable.Where(context.BPR_Persons, x => x.CertificateNum == SSN).FirstOrDefault();
                }
                if (Queryable.Where(context.BPR_Persons, x => x.CertificateNum == Encodings.GetUTF8string(SSN)).FirstOrDefault() != null)
                {
                    return Queryable.Where(context.BPR_Persons, x => x.CertificateNum == Encodings.GetUTF8string(SSN)).FirstOrDefault();
                }
                return null;
            }
        }
        public BPR_Persons GetaVVPersonByDoc(List<string> documents)
        {
            using (var context = new Acra4DbContext(_acra4DbContextOptions))
            {
                foreach (var item in documents)
                {
                    if (Queryable.Where(context.BPR_Documents, x => x.DocumentNumber == item).FirstOrDefault() != null)
                    {
                        long avvPersonId = Queryable.Where(context.BPR_Documents, x => x.DocumentNumber == item).FirstOrDefault().AVVPersonID;
                        return Queryable.Where(context.BPR_Persons, x => x.ID == avvPersonId).FirstOrDefault();
                    }
                }
                return null;
            }
        }
        public int Compare(BPR_Persons aVVPerson, Person person)
        {
            List<string> passports = GetPassportsAndIDCard(person.PersonId);
            List<string> aVVPassports = GetAVVDocuments(aVVPerson.ID);
            if (person.SocialCard != null)
            {
                if (aVVPerson.LastName == null) {
                    aVVPerson.LastName = "";
                }
                if (aVVPerson.FirstName == null)
                {
                    aVVPerson.FirstName = "";
                }
                ////////4.2.21.1
                if ((person.SocialCard == aVVPerson.PNum || (aVVPerson.CertificateNum != null && person.SocialCard == aVVPerson.CertificateNum) ||
                 (aVVPerson.CertificateNum != null && Encodings.GetUTF8string(person.SocialCard).ToUpper() == aVVPerson.CertificateNum.ToUpper()))
                 && GetFirstName(person.FirstName).ToUpper() == aVVPerson.FirstName.ToUpper() &&
                GetLastName(person.LastName).ToUpper() == aVVPerson.LastName.ToUpper())
                {
                    return 1;
                }

                ////////4.2.21.2

                bool isMatching2 = true;
                if (person.SocialCard != aVVPerson.PNum && aVVPerson.CertificateNum != null
                    && person.SocialCard != aVVPerson.CertificateNum &&
                     Encodings.GetUTF8string(person.SocialCard).ToUpper() != aVVPerson.CertificateNum.ToUpper())
                    isMatching2 = false;
                if (!passports.Intersect(aVVPassports).Any())
                    isMatching2 = false;
                if (GetFirstName(person.FirstName).ToUpper() != aVVPerson.FirstName.ToUpper() &&
                    GetLastName(person.LastName).ToUpper() != aVVPerson.LastName.ToUpper())
                    isMatching2 = false;
                if (isMatching2)
                    return 2;

            }
            /////////4.2.21.3
            bool isMatching3 = true;
            if (!passports.Intersect(aVVPassports).Any())
                isMatching3 = false;
            if (GetFirstName(person.FirstName).ToUpper() != aVVPerson.FirstName.ToUpper() ||
                GetLastName(person.LastName).ToUpper() != aVVPerson.LastName.ToUpper())
                isMatching3 = false;
            if (isMatching3)
                return 3;
            return 0;
        }
        public int Compare(AcraIDServices.Models.AVV.BPR_Persons aVVPerson, Person person)
        {
            List<string> passports = GetPassportsAndIDCard(person.PersonId);
            List<string> aVVPassports = new List<string>();
            foreach (var item in aVVPerson.AVVDocuments.Document)
            {
                aVVPassports.Add(item.Document_Number);
            }
            if (person.SocialCard != null)
            {
                ////////4.2.21.1
                if ((person.SocialCard == aVVPerson.PNum || (aVVPerson.Certificate_Number != null && person.SocialCard == aVVPerson.Certificate_Number) ||
                 (aVVPerson.Certificate_Number != null && Encodings.GetUTF8string(person.SocialCard).ToUpper() == aVVPerson.Certificate_Number.ToUpper()))
                 && GetFirstName(person.FirstName).ToUpper() == aVVPerson.AVVDocuments.Document[0].Person.First_Name.ToUpper() &&
                GetLastName(person.LastName).ToUpper() == aVVPerson.AVVDocuments.Document[0].Person.Last_Name.ToUpper())
                {
                    return 1;
                }
                ////////4.2.21.2

                bool isMatching2 = true;
                if (person.SocialCard != aVVPerson.PNum && aVVPerson.Certificate_Number != null
                    && person.SocialCard != aVVPerson.Certificate_Number &&
                     Encodings.GetUTF8string(person.SocialCard).ToUpper() != aVVPerson.Certificate_Number.ToUpper())
                    isMatching2 = false;
                if (!passports.Intersect(aVVPassports).Any())
                    isMatching2 = false;
                if (GetFirstName(person.FirstName).ToUpper() != aVVPerson.AVVDocuments.Document[0].Person.First_Name.ToUpper() &&
                    GetLastName(person.LastName).ToUpper() != aVVPerson.AVVDocuments.Document[0].Person.Last_Name.ToUpper())
                    isMatching2 = false;
                if (isMatching2)
                    return 2;

            }
            /////////4.2.21.3
            bool isMatching3 = true;
            if (!passports.Intersect(aVVPassports).Any())
                isMatching3 = false;
            if (GetFirstName(person.FirstName).ToUpper() != aVVPerson.AVVDocuments.Document[0].Person.First_Name.ToUpper() ||
                GetLastName(person.LastName).ToUpper() != aVVPerson.AVVDocuments.Document[0].Person.Last_Name.ToUpper())
                isMatching3 = false;
            if (isMatching3)
                return 3;
            return 0;
        }
        public int Compare(Person person1, Person person2)
        {
            try
            {
                List<string> passports1 = GetPassportsAndIDCard(person1.PersonId);
                List<string> passports2 = GetPassportsAndIDCard(person2.PersonId);

                /////////4.2.21.3
                bool isMatching3 = true;
                if (!passports1.Intersect(passports2).Any())
                    isMatching3 = false;
                if (GetFirstName(person1.FirstName).ToUpper() != GetFirstName(person2.FirstName).ToUpper() ||
                    GetLastName(person1.LastName).ToUpper() != GetLastName(person2.LastName).ToUpper())
                    isMatching3 = false;
                if (isMatching3)
                    return 3;

                ////////////4.2.21.4
                bool isMatching4 = true;
                if (GetFirstName(person1.FirstName).ToUpper() != GetFirstName(person2.FirstName).ToUpper() ||
                    GetLastName(person1.LastName).ToUpper() != GetLastName(person2.LastName).ToUpper() ||
                    person1.BirthDate != person2.BirthDate)
                    isMatching4 = false;
                if(isMatching4)
                    return 4;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 0;
            }
            
            return 0;
        }
        public int Compare(AcraData.Models.Acra3.Organization org1, AcraData.Models.Acra3.Organization org2)
        {
            if (org1.Hvhh == org2.Hvhh)
                return 5;
            return 0;
        }
        public string GetFirstName(uint? firstNameId)
        {
            Console.WriteLine($"firstNameId: {firstNameId}");

            if (firstNameId == null)
            {
                return null;
            }
            using (var context = new Acra3DbContext(_acra3DbContextOptions))
            {
                string nameStr = Queryable.Where(context.DicFirstNames, x => x.FirstNameId == Convert.ToInt32(firstNameId)).FirstOrDefault().FirstName;
                return Encodings.GetUTF8string(nameStr);
            }
        }
        public string GetLastName(uint? LastNameId)
        {
            if (LastNameId == null)
            {
                return null;
            }
            using (var context = new Acra3DbContext(_acra3DbContextOptions))
            {
                string nameStr = Queryable.Where(context.DicLastNames, x => x.LastNameId == Convert.ToInt32(LastNameId)).FirstOrDefault().LastName;
                return Encodings.GetUTF8string(nameStr);
            }
        }
        public List<string> GetPassportsAndIDCard(uint personID)
        {
            List<Passport> passports = new List<Passport>();
            List<string> passportStrs = new List<string>();
            string IDCard;
            using (var context = new Acra3DbContext(_acra3DbContextOptions))
            {   
                passports = Queryable.Where(context.Passports, x => x.PersonId == Convert.ToInt32(personID)).ToList();
                IDCard = Queryable.Where(context.IdCards, x => x.PersonId == Convert.ToInt32(personID)).FirstOrDefault()?.IdCardNum;
            }
            foreach (var item in passports)
            {
                passportStrs.Add(item.PassportNum);
            }
            if (IDCard != null)
            {
                passportStrs.Add(IDCard);
            }
            return passportStrs;
        }
        public List<string> GetAVVDocuments(long avvPersonID)
        {
            List<BPR_Documents> documents = new List<BPR_Documents>();
            List<string> documentStrs = new List<string>();
            using (var context = new Acra4DbContext(_acra4DbContextOptions))
            {
                documents = Queryable.Where(context.BPR_Documents, x => x.AVVPersonID == avvPersonID).ToList();
            }
            foreach (var item in documents)
            {
                documentStrs.Add(item.DocumentNumber);
            }
            return documentStrs;
        }
        public List<Person> GetAllPersonsByDoc(string document, uint? personId)
        {
            List<Passport> passports = new List<Passport>();
            List<int?> personIds = new List<int?>();
            List<Person> persons = new List<Person>();
            using (var context = new Acra3DbContext(_acra3DbContextOptions))
            {
                passports = Queryable.Where(context.Passports, x => x.PassportNum == document).ToList();
                foreach (var item in passports)
                {
                    if (item.PersonId != personId)
                    {
                        personIds.Add(item.PersonId);

                    }
                }
                if (Queryable.Where(context.IdCards, x => x.IdCardNum == document).FirstOrDefault() != null)
                {
                    personIds.Add(Queryable.Where(context.IdCards, x => x.IdCardNum == document).FirstOrDefault().PersonId);
                }
                foreach (var item in personIds)
                {
                    persons.Add(Queryable.Where(context.Persons, x => x.PersonId == Convert.ToUInt32(item)).FirstOrDefault());
                }
            }
            return persons;
        }
        private long? PersonIDExistsinAvvMapper(uint? personID)
        {
            using (var context = new Acra3DbContext(_acra3DbContextOptions))
            {
                ACRAID_MAPPER acraidMapper = Queryable.Where(context.ACRAID_MAPPER, x => x.PersonID == personID).FirstOrDefault();
                if (acraidMapper != null && acraidMapper.ACRAID.Value.ToString().Length == 12)
                {
                    return acraidMapper.ACRAID;
                }
                else {
                    return null;
                }
               /* if (context.ACRAID_MAPPER.Where(x => x.PersonID == personID).FirstOrDefault() != null)
                {
                    return context.ACRAID_MAPPER.Where(x => x.PersonID == personID).FirstOrDefault().ACRAID;
                }
                else
                {
                    return null;
                } */
            }

        }
        private void Get3rdSourceInfoBySSN(string SSN)
        {
            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   7" + Environment.NewLine);
            //_logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            try
            {
                if (SSN != "")
                {
                    AcraIDServices.Models.AVV.BySSN personData = new AcraIDServices.Models.AVV.BySSN
                    {
                        psn = SSN,
                        Addresses = AcraIDServices.Models.AVV.Addresses.CURRENT
                    };
                    var url = $"{_configuration.EkengServiceURL}/AVV/GetPersonInfoBySSN";

                    var client = new HttpClient();
                    client.Timeout = new TimeSpan(0, 1, 0);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(personData), Encoding.UTF8, "application/json")
                    };
                    HttpResponseMessage response = client.SendAsync(request).Result;

                    Log3rdSourceRequests(request, response);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            responseModel = JsonConvert.DeserializeObject<AcraIDServices.Models.AVV.AvvResponse>(response.Content.ReadAsStringAsync().Result);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                    {
                        isEkengSuccess = false;
                    }

                }

            }
            catch (Exception ex)
            {
                isEkengSuccess = false;
                //_logger.Log.ErrorFormat("Get3rdSourceInfoByDoc:{0}", ex.Message);
            }
            //_logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
        }
        private void Get3rdSourceInfoByDoc(string Document)
        {
            //_logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            try
            {
                if (Document != "")
                {
                    AcraIDServices.Models.AVV.ByDocument personData = new AcraIDServices.Models.AVV.ByDocument
                    {
                        docnum = Document,
                        Addresses = AcraIDServices.Models.AVV.Addresses.CURRENT
                    };
                    var url = $"{_configuration.EkengServiceURL}/AVV/GetPersonInfoByDocument";

                    var client = new HttpClient();
                    client.Timeout = new TimeSpan(0, 1, 0);
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(personData), Encoding.UTF8, "application/json")
                    };
                    HttpResponseMessage response = client.SendAsync(request).Result;
                    Log3rdSourceRequests(request, response);
                    //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} ssn = {Document}" + Environment.NewLine);
                    //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} response = {response.IsSuccessStatusCode}" + Environment.NewLine);
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            responseModel = JsonConvert.DeserializeObject<AcraIDServices.Models.AVV.AvvResponse>(response.Content.ReadAsStringAsync().Result);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    else
                        isEkengSuccess = false;
                }
            }
            catch (Exception ex)
            {
                isEkengSuccess = false;
                //_logger.Log.ErrorFormat("Get3rdSourceInfoByDoc:{0}", ex.Message);
            }
            //_logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
        }
        private void Log3rdSourceRequests(HttpRequestMessage requestMessage, HttpResponseMessage responseMessage)
        {
            // System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   8" + Environment.NewLine);
            //_logger.Log.Info($"Start Method: {MethodBase.GetCurrentMethod().Name}");
            using (var context = new AcraData.Data.AcraJournalDbContext(_acraJournalDbContextOptions))
            {
                try
                {
                    context.BPR_Transaction.Add(new BPR_Transaction() { Request = requestMessage.Content.ReadAsStringAsync().Result, Response = responseMessage.Content.ReadAsStringAsync().Result, ResponseDateTime = DateTime.Now });
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    //_logger.Log.Error($"Error Method: {MethodBase.GetCurrentMethod().Name} Error:{ex.Message}");
                }
            }
            //_logger.Log.Info($"End Method: {MethodBase.GetCurrentMethod().Name}");
        }
        public void SendEmail(string txt)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(_configuration.SMTPClient);
                mail.From = new MailAddress(_configuration.SendErrorsFromEmail);
                List<string> toMails = _configuration.SendErrorsToEmail.Split(',').ToList();
                foreach (var item in toMails)
                {
                    mail.To.Add(item);
                }
                mail.Subject = "AcraId Service Error";
                mail.Body = txt;
                SmtpServer.Port = 25;
                SmtpServer.Credentials = new System.Net.NetworkCredential("dev.support@acra.am", "Dev$123");
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
            }
        }

        private bool validateTime(string dateInString)
        {
            try
            {
                string tmp = dateInString.Substring(5, 5);
                string secondPart = Encoding.UTF8.GetString(Encoding.Default.GetBytes(tmp));
                if (secondPart == "00-00") {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                return false;
            }  
        }

        private List<Person> GetAllPersonsBySameNameAndBirthDate(Person person)
        {
            List<Person> persons = new List<Person>();
            using (var context = new Acra3DbContext(_acra3DbContextOptions))
            {
                persons.AddRange(Queryable.Where(context.Persons, x => x.FirstName == person.FirstName && x.LastName == person.LastName && x.BirthDate == person.BirthDate).ToList());
            }

            return persons;
        }
    }
}
