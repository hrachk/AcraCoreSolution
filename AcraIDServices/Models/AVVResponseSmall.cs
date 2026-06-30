using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AcraIDServices.Models.AVV
{
    public partial class AVVResponseSmall
    {
        [JsonPropertyName("persons")]
        public List<Person> Persons { get; set; }

    }
    public partial class  Persons
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("acraid")]
        public object Acraid { get; set; }

        [JsonPropertyName("pNum")]
        public string PNum { get; set; }

        [JsonPropertyName("ssnIndicator")]
        public bool SsnIndicator { get; set; }

        [JsonPropertyName("certificateNum")]
        public object CertificateNum { get; set; }

        [JsonPropertyName("isDead")]
        public bool IsDead { get; set; }

        [JsonPropertyName("deathDate")]
        public DateTime DeathDate { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; }

        [JsonPropertyName("avvGetDate")]
        public DateTime AvvGetDate { get; set; }

        [JsonPropertyName("bpR_Documents")]
        public List<Document> BpRDocuments { get; set; }
    }

    public partial class Document
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("avvPersonID")]
        public int AvvPersonID { get; set; }

        [JsonPropertyName("photo")]
        public string Photo { get; set; }

        [JsonPropertyName("documentType")]
        public int DocumentType { get; set; }

        [JsonPropertyName("documentNumber")]
        public string DocumentNumber { get; set; }

        [JsonPropertyName("documentStatus")]
        public string DocumentStatus { get; set; }

        [JsonPropertyName("documentDepartment")]
        public string DocumentDepartment { get; set; }

        [JsonPropertyName("countryName")]
        public object CountryName { get; set; }

        [JsonPropertyName("countryCode")]
        public object CountryCode { get; set; }

        [JsonPropertyName("issuanceDate")]
        public DateTime IssuanceDate { get; set; }

        [JsonPropertyName("validityDate")]
        public DateTime ValidityDate { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("middleName")]
        public string MiddleName { get; set; }

        [JsonPropertyName("englishLastName")]
        public string EnglishLastName { get; set; }

        [JsonPropertyName("englishFirstName")]
        public string EnglishFirstName { get; set; }

        [JsonPropertyName("englishMiddleName")]
        public string EnglishMiddleName { get; set; }

        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("gender")]
        public int Gender { get; set; }

        [JsonPropertyName("avvGetDateTime")]
        public DateTime AvvGetDateTime { get; set; }
    }
}
