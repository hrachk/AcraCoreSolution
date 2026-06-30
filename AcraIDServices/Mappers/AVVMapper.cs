using AcraData.Data;
using AcraData.Models.Acra4;
using AcraIDServices.Models;
using AcraUtils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AcraIDServices.Mappers
{
    public class AVVMapper : IAVVMapper
    {
        private Logger _logger;
        private DbContextOptions<AcraData.Data.Acra4DbContext> _acra4DbContextOptions;
        //  private DataHelpers _dataHelpers;

        public AVVMapper(Logger logger, DbContextOptions<AcraData.Data.Acra4DbContext> acra4DbContextOptions)
        {
            _logger = logger;
            _acra4DbContextOptions = acra4DbContextOptions;
            //     _dataHelpers = new DataHelpers(_acra4DbContextOptions, _logger);
        }

        public BPR_Persons ImportPerson(PDataModel pDataModel)
        {


            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbContextOptions))
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    //context.ChangeTracker.AutoDetectChangesEnabled = false;
                    try
                    {
                        /*AVVPERSON*/
                        BPR_Persons person = context.BPR_Persons.FirstOrDefault(p => p.PNum == pDataModel.PassportData.PNum) ?? new BPR_Persons();

                        person.PNum = pDataModel.PassportData.PNum;
                        person.SSNIndicator = pDataModel.PassportData.SsnIndicator;
                        person.CertificateNum = pDataModel.PassportData.CertificateNum;
                        person.IsDead = pDataModel.PassportData.IsDead;
                        person.DeathDate = (pDataModel.PassportData.DeathDate != null && !pDataModel.PassportData.DeathDate.Contains("00/00")) ? DateTime.ParseExact(pDataModel.PassportData.DeathDate, "dd/MM/yyyy", null) : default(DateTime);
                        person.FirstName = pDataModel.FirstName;
                        person.LastName = pDataModel.LastName;
                        person.BirthDate = (pDataModel.PassportData.AvvDocuments.AvvDocument.OrderByDescending(d => d.IssuanceDate).First().BirthDate != null
                                        && !pDataModel.PassportData.AvvDocuments.AvvDocument.OrderByDescending(d => d.IssuanceDate).First().BirthDate.Contains("00/00")) ? DateTime.ParseExact(pDataModel.PassportData.AvvDocuments.AvvDocument.OrderByDescending(d => d.IssuanceDate).First().BirthDate, "dd/MM/yyyy", null) : default(DateTime);
                        person.Gender = pDataModel.PassportData.AvvDocuments.AvvDocument.OrderByDescending(d => d.IssuanceDate).First().Gender;

                        person.AVVGetDate = DateTime.Now;
                        context.AddOrUpdate(person);
                        //context.Update(person);

                        context.SaveChanges();

                        var personID = context.BPR_Persons.First(p => p.PNum == pDataModel.PassportData.PNum).ID;

                        /*AVVADDRESS*/
                        var address = context.BPR_Addresses.FirstOrDefault(a => a.AVVPersonID == personID) ?? new AcraData.Models.Acra4.BPR_Addresses();
                        if (pDataModel.PassportData.AvvRegistrationAddress != null)
                            EkengAddressMapper.Map(pDataModel.PassportData.AvvRegistrationAddress, ref address);
                        address.AVVPersonID = personID;

                        context.AddOrUpdate(address);

                        context.SaveChanges();

                        /*AVVDOCUMENTS*/
                        var documents = System.Linq.Queryable
    .Where(context.BPR_Documents, d => d.AVVPersonID == personID)
    .ToList();
                        if (documents.Count == 0)
                            return null;

                        var documentTypes = context.BPR_DocumentTypes
    .ToDictionary(t => t.DocumentType, t => t.ID);

                        if (documents != null)
                        {
                            foreach (var oldDocument in documents)
                            {
                                bool existsInNew = pDataModel.PassportData.AvvDocuments.AvvDocument.Any(n =>
                                    documentTypes.TryGetValue(n.DocumentIdentifier.DocumentType, out var typeId) &&
                                    typeId == oldDocument.DocumentType &&
                                    n.DocumentIdentifier.DocumentNumber == oldDocument.DocumentNumber);

                                if (!existsInNew)
                                {
                                    context.BPR_Documents.Remove(oldDocument);
                                }
                            }

                            context.SaveChanges();
                        }

                        foreach (var document in pDataModel.PassportData.AvvDocuments.AvvDocument)
                        {

                            var documentTypeId = context.BPR_DocumentTypes
      .FirstOrDefault(t => t.DocumentType == document.DocumentIdentifier.DocumentType)
      ?.ID;

                            var genderId = context.BPR_Genders
                                .FirstOrDefault(t => t.Gender == document.Gender)
                                ?.ID;


                            var avvDocument = person.BPR_Documents.Where(d => d.AVVPersonID == person.ID && d.DocumentType == documentTypeId && d.DocumentNumber == document.DocumentIdentifier.DocumentNumber).FirstOrDefault() ?? new AcraData.Models.Acra4.BPR_Documents();
                            EkengDocumentMapper.Map(document, documentTypeId.Value, genderId.Value, ref avvDocument);
                            avvDocument.AVVPersonID = personID;

                            context.AddOrUpdate(avvDocument);

                            context.SaveChanges();

                        }
                        tx.Commit();
                        return person;
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        _logger.Log.Fatal("AVVMapper.Import:", ex);
                        return null;
                    }
                }
            }
        }

        public BPR_Persons ImportPerson(Models.AVV.BPR_Persons pDataModel)
        {
            using (var context = new AcraData.Data.Acra4DbContext(_acra4DbContextOptions))
            {
                using (var tx = context.Database.BeginTransaction())
                {
                    //context.ChangeTracker.AutoDetectChangesEnabled = false;
                    try
                    {
                        /*AVVPERSON*/
                        BPR_Persons person = context.BPR_Persons.FirstOrDefault(p => p.PNum.Equals(pDataModel.PNum)) ?? new BPR_Persons();
                        person.PNum = pDataModel.PNum;
                        person.SSNIndicator = pDataModel.SSN_Indicator;
                        person.CertificateNum = pDataModel.Certificate_Number;
                        person.IsDead = pDataModel.IsDead;
                        person.DeathDate = (pDataModel.DeathDate != null && !pDataModel.DeathDate.ToString().Contains("00/00")) ? DateTime.ParseExact(pDataModel.DeathDate.ToString(), "dd/MM/yyyy", null) : default(DateTime);


                        var lastDocument = pDataModel.AVVDocuments.Document.Where(p => p.PassportData != null).Where(x => x.PassportData.Passport_Issuance_Date != null).
                            OrderByDescending(d => DateTime.ParseExact(d.PassportData.Passport_Issuance_Date, "dd/MM/yyyy", null)).First();
                        person.FirstName = lastDocument.Person.First_Name;
                        person.LastName = lastDocument.Person.Last_Name;
                        person.BirthDate = (lastDocument.Person.Birth_Date != null && !lastDocument.Person.Birth_Date.ToString().Contains("00/00")) ? DateTime.ParseExact(lastDocument.Person.Birth_Date.ToString(), "dd/MM/yyyy", null) : DateTime.ParseExact(lastDocument.Person.Birth_Date.ToString().Replace("00/00", "01/01"), "dd/MM/yyyy", null);
                        person.Gender = lastDocument.Person.Genus;
                        person.AVVGetDate = DateTime.Now;
                        context.AddOrUpdate(person);


                        context.SaveChanges();
                        var personID = person.ID;

                        /*AVVADDRESS*/
                        var addresses = System.Linq.Queryable
                      .Where(context.BPR_Addresses, d => d.AVVPersonID == personID)
                      .ToList();
                        if (addresses != null && addresses.Count > 0)
                        {
                            foreach (var oldAddress in addresses)
                            {
                                
                                context.BPR_Addresses.Remove(oldAddress);
                            }

                            context.SaveChanges();
                        }

                        if (pDataModel.AVVAddresses != null && pDataModel.AVVAddresses.AVVAddresses != null)
                        {
                            foreach (var address in pDataModel.AVVAddresses.AVVAddresses)
                            {

                                var avvAddress = context.BPR_Addresses.FirstOrDefault(a => a.AVVPersonID == personID) ?? new AcraData.Models.Acra4.BPR_Addresses();
                                AVVAddressMapper.Map(address, ref avvAddress);
                                avvAddress.AVVPersonID = personID;
                                context.AddOrUpdate(avvAddress);

                                context.SaveChanges();

                            }
                        }

                        /*AVVDOCUMENTS*/
                        List<AcraData.Models.Acra4.BPR_Documents> documents = System.Linq.Queryable.Where(context.BPR_Documents, d => d.AVVPersonID == personID).ToList();

                        // Загружаем все DocumentTypes в словарь один раз
                        var documentTypeMap = context.BPR_DocumentTypes
                            .ToDictionary(t => t.DocumentType, t => t.ID);


                        if (documents != null && documents.Count > 0)
                        {
                            // Удаляем старые документы, которых нет в новых данных
                            foreach (var oldDocument in documents)
                            {
                                bool existsInNew = pDataModel.AVVDocuments.Document.Any(n =>
                                    documentTypeMap.TryGetValue(n.Document_Type, out var typeId) &&
                                    typeId == oldDocument.DocumentType &&
                                    n.Document_Number == oldDocument.DocumentNumber);

                                if (!existsInNew)
                                {
                                    context.BPR_Documents.Remove(oldDocument);
                                }
                            }


                            context.SaveChanges();
                        }

                        // --- Загружаем справочники один раз для всего цикла ---
                         

                        var genderMap = context.BPR_Genders
                            .ToDictionary(g => g.Gender, g => g.ID);

                        foreach (var document in pDataModel.AVVDocuments.Document)
                        {
                            // Получаем ID безопасно
                            if (!documentTypeMap.TryGetValue(document.Document_Type, out var documentTypeId))
                                throw new InvalidOperationException($"DocumentType not found: {document.Document_Type}");

                            if (!genderMap.TryGetValue(document.Person.Genus, out var genderId))
                                throw new InvalidOperationException($"Gender not found: {document.Person.Genus}");

                            // Ищем существующий документ
                            var avvDocument = person.BPR_Documents
                                .FirstOrDefault(d =>
                                    d.AVVPersonID == person.ID &&
                                    d.DocumentType == documentTypeId &&
                                    d.DocumentNumber == document.Document_Number)
                                ?? new AcraData.Models.Acra4.BPR_Documents
                                {
                                    AVVPersonID = personID
                                };

                            AVVDocumentMapper.Map(document, documentTypeId, genderId, ref avvDocument);

                            avvDocument.AVVGetDateTime = DateTime.Now;

                            context.BPR_Documents.Update(avvDocument); // EF сам определит add или update
                        }

                        // Один вызов SaveChanges после всех документов
                        context.SaveChanges();

                        //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   14" + Environment.NewLine);
                        tx.Commit();
                        //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now}   15" + Environment.NewLine);
                        return person;
                    }
                    catch (Exception ex)
                    {
                        tx.Rollback();
                        _logger.Log.Fatal("AVVMapper.Import:", ex);
                        return null;
                    }
                }
            }
        }
    }

    public static class EkengDocumentMapper
    {
        public static void Map(AvvDocument document, int documentTypeId, int genderId, ref AcraData.Models.Acra4.BPR_Documents newDocument)
        {
            newDocument.DocumentType = documentTypeId;
            newDocument.DocumentNumber = document.DocumentIdentifier.DocumentNumber;
            newDocument.DocumentDepartment = document.DocumentDepartment;
            if (document.Citizenship != null)
            {
                newDocument.CountryName = document.Citizenship.CountryName;
                newDocument.CountryCode = document.Citizenship.CountryCode;
            }
            newDocument.IssuanceDate = (document.IssuanceDate != null && !document.IssuanceDate.Contains("00/00")) ? DateTime.ParseExact(document.IssuanceDate, "dd/MM/yyyy", null) : default(DateTime);
            newDocument.ValidityDate = (document.ValidityDate != null && !document.ValidityDate.Contains("00/00")) ? DateTime.ParseExact(document.ValidityDate, "dd/MM/yyyy", null) : default(DateTime);
            newDocument.LastName = document.LastName;
            newDocument.FirstName = document.FirstName;
            newDocument.MiddleName = document.MiddleName;
            newDocument.EnglishLastName = document.EnglishLastName;
            newDocument.EnglishFirstName = document.EnglishFirstName;
            newDocument.EnglishMiddleName = document.EnglishMiddleName;
            newDocument.BirthDate = (document.BirthDate != null && !document.BirthDate.Contains("00/00")) ? DateTime.ParseExact(document.BirthDate, "dd/MM/yyyy", null) : default(DateTime);
            newDocument.Gender = genderId;

            ////

            ////
        }
    }

    public static class EkengAddressMapper
    {
        public static void Map(AvvRegistrationAddress address, ref AcraData.Models.Acra4.BPR_Addresses newAddress)
        {
            newAddress.LocationCode = address.LocationCode ?? string.Empty;
            newAddress.Region = address.Region ?? string.Empty;
            newAddress.Community = address.Community ?? string.Empty;
            newAddress.Residence = address.Residence ?? string.Empty;
            newAddress.Street = address.Street ?? string.Empty;
            newAddress.Building = address.Building ?? string.Empty;
            newAddress.BuildingType = address.BuildingType ?? string.Empty;
            newAddress.Apartment = address.Apartment ?? string.Empty;
        }
    }

    public static class AVVDocumentMapper
    {
        public static void Map(Models.AVV.Document document, int documentTypeId, int genderId, ref AcraData.Models.Acra4.BPR_Documents newDocument)
        {

            newDocument.DocumentType = documentTypeId;
            newDocument.DocumentNumber = document.Document_Number;
            newDocument.DocumentStatus = document.Document_Status;
            newDocument.DocumentDepartment = document.Document_Department;
            if (document.Person.Citizenship != null && document.Person.Citizenship._Citizenship != null)
            {
                newDocument.CountryName = document.Person.Citizenship._Citizenship.FirstOrDefault().CountryName;
                newDocument.CountryCode = document.Person.Citizenship._Citizenship.FirstOrDefault().CountryCode;
            }

            newDocument.IssuanceDate = (document.PassportData != null && document.PassportData.Passport_Issuance_Date != null && !document.PassportData.Passport_Issuance_Date.ToString().Contains("00/00")) ? DateTime.ParseExact(document.PassportData.Passport_Issuance_Date.ToString(), "dd/MM/yyyy", null) : default(DateTime);
            newDocument.ValidityDate = (document.PassportData != null && document.PassportData.Passport_Validity_Date != null && !document.PassportData.Passport_Validity_Date.ToString().Contains("00/00")) ? DateTime.ParseExact(document.PassportData.Passport_Validity_Date.ToString(), "dd/MM/yyyy", null) : default(DateTime);
            newDocument.LastName = document.Person.Last_Name;
            newDocument.FirstName = document.Person.First_Name;
            newDocument.MiddleName = document.Person.Patronymic_Name;
            newDocument.EnglishLastName = document.Person.English_Last_Name;
            newDocument.EnglishFirstName = document.Person.English_First_Name;
            newDocument.EnglishMiddleName = document.Person.English_Patronymic_Name;
            newDocument.BirthDate = (document.Person.Birth_Date != null && !document.Person.Birth_Date.ToString().Contains("00/00")) ? DateTime.ParseExact(document.Person.Birth_Date.ToString(), "dd/MM/yyyy", null) : default(DateTime);
            newDocument.Gender = genderId;
            newDocument.Photo = document.Photo_ID;
        }
    }

    public static class AVVAddressMapper
    {
        public static void Map(Models.AVV.AVVAddress address, ref AcraData.Models.Acra4.BPR_Addresses newAddress)
        {
            newAddress.LocationCode = address.RegistrationAddress.LocationCode ?? string.Empty;
            newAddress.Region = address.RegistrationAddress.Region ?? string.Empty;
            newAddress.Community = address.RegistrationAddress.Community ?? string.Empty;
            newAddress.Residence = address.RegistrationAddress.Residence ?? string.Empty;
            newAddress.Street = address.RegistrationAddress.Street ?? string.Empty;
            newAddress.Building = address.RegistrationAddress.Building ?? string.Empty;
            newAddress.BuildingType = address.RegistrationAddress.Building_Type ?? string.Empty;
            newAddress.Apartment = address.RegistrationAddress.Apartment ?? string.Empty;
        }
    }
}


