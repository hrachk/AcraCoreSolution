using Newtonsoft.Json;
using System.Collections.Generic;

namespace AcraIDServices.Models.AVV
{
    public partial class AvvResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("result")]
        public List<BPR_Persons> Result { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public partial class BPR_Persons
    {
        [JsonProperty("PNum")]
        public string PNum { get; set; }

        [JsonProperty("SSN_Indicator")]
        public bool SSN_Indicator { get; set; }

        [JsonProperty("Certificate_Number")]
        public string Certificate_Number { get; set; }

        [JsonProperty("IsDead")]
        public bool IsDead { get; set; }

        [JsonProperty("DeathDate")]
        public string DeathDate { get; set; } // Date_AVV

        [JsonProperty("AvvDocuments")]
        public BPR_Documents AVVDocuments { get; set; }

        [JsonProperty("AVVAddresses")]
        public Bpr_Addresses AVVAddresses { get; set; }

        [JsonProperty("citizenshipStoppedDate")]
        public string CitizenshipStoppedDate { get; set; } //Date_AVV
    }

    public partial class Bpr_Addresses
    {
        [JsonProperty("AVVAddresses")]
        public List<AVVAddress> AVVAddresses { get; set; }
    }

    public partial class AVVAddress
    {
        [JsonProperty("RegistrationAddress")]
        public RegistrationAddress RegistrationAddress { get; set; }

        [JsonProperty("residenceDocument")]
        public ResidenceDocument ResidenceDocument { get; set; }

        [JsonProperty("registrationData")]
        public RegistrationData RegistrationData { get; set; }
    }

    public partial class ResidenceDocument
    {
        [JsonProperty("Residence_Document_Type")]
        public string Residence_Document_Type { get; set; }

        [JsonProperty("Residence_Document_Number")]
        public string Residence_Document_Number { get; set; }

        [JsonProperty("Residence_Document_Department")]
        public string Residence_Document_Department { get; set; }

        [JsonProperty("Residence_Document_Date")]
        public string Residence_Document_Date { get; set; }

        [JsonProperty("Residence_Document_Validity_Date")]
        public string Residence_Document_Validity_Date { get; set; }
    }

    public partial class RegistrationAddress
    {
        [JsonProperty("LocationCode")]
        public string LocationCode { get; set; }

        [JsonProperty("Region")]
        public string Region { get; set; }

        [JsonProperty("community")]
        public string Community { get; set; }

        [JsonProperty("Residence")]
        public string Residence { get; set; }

        [JsonProperty("Street")]
        public string Street { get; set; }

        [JsonProperty("Building")]
        public string Building { get; set; }

        [JsonProperty("Building_Type")]
        public string Building_Type { get; set; }

        [JsonProperty("Apartment")]
        public string Apartment { get; set; }
    }

    public partial class RegistrationData
    {
        [JsonProperty("Registration_Department")]
        public string Registration_Department { get; set; }

        [JsonProperty("Registration_Date")]
        public string Registration_Date { get; set; }

        [JsonProperty("Registration_Type")]
        public string Registration_Type { get; set; }

        [JsonProperty("Registration_Status")]
        public string Registration_Status { get; set; }

        [JsonProperty("Temporary_Registration_Date")]
        public string Temporary_Registration_Date { get; set; } //Date_AVV

        [JsonProperty("Registration_Aim")]
        public RegistrationAim Registration_Aim { get; set; }

        [JsonProperty("unRegistrationAim")]
        public RegistrationAim UnRegistrationAim { get; set; }

        [JsonProperty("registeredDate")]
        public string RegisteredDate { get; set; } //Date_AVV

        [JsonProperty("registeredDepartment")]
        public string RegisteredDepartment { get; set; }
    }

    public partial class RegistrationAim
    {
        [JsonProperty("AimName")]
        public string AimName { get; set; }

        [JsonProperty("AimCode")]
        public string AimCode { get; set; }
    }

    public partial class BPR_Documents
    {
        [JsonProperty("document")]
        public List<Document> Document { get; set; }
    }

    public partial class Document
    {
        [JsonProperty("Photo_ID")]
        public string Photo_ID { get; set; }

        [JsonProperty("Document_Status")]
        public string Document_Status { get; set; }

        [JsonProperty("Document_Type")]
        public string Document_Type { get; set; }

        [JsonProperty("Document_Number")]
        public string Document_Number { get; set; }

        [JsonProperty("Other_DocumentType")]
        public string Other_DocumentType { get; set; }

        [JsonProperty("Document_Department")]
        public string Document_Department { get; set; }

        [JsonProperty("BasicDocument")]
        public BasicDocument BasicDocument { get; set; }

        [JsonProperty("Person")]
        public Person Person { get; set; }

        [JsonProperty("PresidentOrder")]
        public PresidentOrder PresidentOrder { get; set; }

        [JsonProperty("passportData")]
        public PassportData PassportData { get; set; }
    }

    public partial class BasicDocument
    {
        [JsonProperty("Basic_Document_Code")]
        public string Basic_Document_Code { get; set; }

        [JsonProperty("Basic_Document_Name")]
        public string Basic_Document_Name { get; set; }

        [JsonProperty("Basic_Document_Number")]
        public string Basic_Document_Number { get; set; }

        [JsonProperty("Basic_Document_Country")]
        public Country Basic_Document_Country { get; set; }
    }

    public partial class PresidentOrder
    {
        [JsonProperty("President_Order")]
        public string President_Order { get; set; }

        [JsonProperty("President_Order_Date")]
        public string President_Order_Date { get; set; }
    }

    public partial class PassportData
    {
        [JsonProperty("Passport_Type")]
        public string Passport_Type { get; set; }

        [JsonProperty("Passport_Issuance_Date")]
        public string Passport_Issuance_Date { get; set; } //Date_AVV

        [JsonProperty("Passport_Validity_Date")]
        public string Passport_Validity_Date { get; set; }  //Date_AVV

        [JsonProperty("Passport_Validity_Date_FC")]
        public string Passport_Validity_Date_FC { get; set; }  //Date_AVV

        [JsonProperty("Passport_Extension_Date")]
        public string Passport_Extension_Date { get; set; } //Date_AVV

        [JsonProperty("Passport_Extension_Department")]
        public string Passport_Extension_Department { get; set; }

        [JsonProperty("Related_Document_Number")]
        public object Related_Document_Number { get; set; }

        [JsonProperty("Related_Document_Date")]
        public string Related_Document_Date { get; set; }

        [JsonProperty("Related_Document_Department")]
        public string Related_Document_Department { get; set; }
    }

    public partial class Person
    {
        [JsonProperty("Nationality")]
        public Nationality Nationality { get; set; }

        [JsonProperty("Citizenship")]
        public Citizenship Citizenship { get; set; }

        [JsonProperty("Last_Name")]
        public string Last_Name { get; set; }

        [JsonProperty("First_Name")]
        public string First_Name { get; set; }

        [JsonProperty("Patronymic_Name")]
        public string Patronymic_Name { get; set; }

        [JsonProperty("Birth_Date")]
        public string Birth_Date { get; set; } //Date_AVV

        [JsonProperty("Genus")]
        public string Genus { get; set; }

        [JsonProperty("English_Last_Name")]
        public string English_Last_Name { get; set; }

        [JsonProperty("English_First_Name")]
        public string English_First_Name { get; set; }

        [JsonProperty("English_Patronymic_Name")]
        public string English_Patronymic_Name { get; set; }

        [JsonProperty("Birth_Country")]
        public Country Birth_Country { get; set; }

        [JsonProperty("Birth_Region")]
        public string Birth_Region { get; set; }

        [JsonProperty("Birth_Community")]
        public string Birth_Community { get; set; }

        [JsonProperty("Birth_Residence")]
        public object Birth_Residence { get; set; }

        [JsonProperty("Birth_Address")]
        public string Birth_Address { get; set; }
    }

    public partial class Country
    {
        [JsonProperty("CountryName")]
        public string CountryName { get; set; }

        [JsonProperty("CountryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("CountryShortName")]
        public string CountryShortName { get; set; }
    }

    public partial class Citizenship
    {
        [JsonProperty("Citizenship")]
        public List<Country> _Citizenship { get; set; }
    }

    public partial class Nationality
    {
        [JsonProperty("NationalityName")]
        public string NationalityName { get; set; }

        [JsonProperty("NationalityCode")]
        public string NationalityCode { get; set; }
    }

    public class Date_AVV
    {
        public string DateAVVFormat = "dd'/'MM'/'yyyy";
        public string DateAVV;

        // public string DateAVV { get { return dateAVV; } set { dateAVV = (Convert.ToDateTime(value).ToString(DateAVVFormat)); } }
    }
}
