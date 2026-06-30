using System;
using System.Collections.Generic;

using System.Globalization;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace AcraIDServices.Models
{

    [XmlRoot(ElementName = "data")]
    public partial class PDataModel
    {
        [XmlElement(ElementName="opaque")]
        public string Opaque { get; set; }

        [XmlElement(ElementName="last_name")]
        public string LastName { get; set; }

        [XmlElement(ElementName="first_name")]
        public string FirstName { get; set; }

        [XmlElement(ElementName="SSN")]
        public string Ssn { get; set; }

        [XmlElement(ElementName="passport_data")]
        public PassportData PassportData { get; set; }

        [XmlElement(ElementName="e_civil")]
        public string ECivil { get; set; }

        [XmlElement(ElementName="vehicle_info")]
        public string VehicleInfo { get; set; }

        [XmlElement(ElementName="driving_license")]
        public string DrivingLicense { get; set; }

        [XmlElement(ElementName="e_register")]
        public string ERegister { get; set; }

        [XmlElement(ElementName="ces_data")]
        public string CesData { get; set; }
    }

    [XmlRoot(ElementName = "passport_data")]
    public partial class PassportData
    {
        [XmlElement(ElementName="PNum")]
        public string PNum { get; set; }

        [XmlElement(ElementName="SSNIndicator")]       
        public bool SsnIndicator { get; set; }

        [XmlElement(ElementName="CertificateNum")]
        public string CertificateNum { get; set; }

        [XmlElement(ElementName="Photo")]
        public string Photo { get; set; }

        [XmlElement(ElementName="IsDead")]        
        public bool IsDead { get; set; }

        [XmlElement(ElementName = "DeathDate")]
        public string DeathDate { get; set; }

        [XmlElement(ElementName="AVVDocuments")]
        public AvvDocuments AvvDocuments { get; set; }

        [XmlElement(ElementName="AVVRegistrationAddress")]
        public AvvRegistrationAddress AvvRegistrationAddress { get; set; }
    }

    [XmlRoot(ElementName = "AVVDocuments")]
    public partial class AvvDocuments
    {
        [XmlElement(ElementName="AVVDocument")]
        public List<AvvDocument> AvvDocument { get; set; }
    }

    [XmlRoot(ElementName = "AvvDocument")]
    public partial class AvvDocument
    {
        [XmlElement(ElementName="DocumentIdentifier")]
        public DocumentIdentifier DocumentIdentifier { get; set; }

        [XmlElement(ElementName="DocumentDepartment")]
        public string DocumentDepartment { get; set; }

        [XmlElement(ElementName="Citizenship")]
        public Citizenship Citizenship { get; set; }

        [XmlElement(ElementName="LastName")]
        public string LastName { get; set; }

        [XmlElement(ElementName="FirstName")]
        public string FirstName { get; set; }

        [XmlElement(ElementName="MiddleName")]
        public string MiddleName { get; set; }

        [XmlElement(ElementName="EnglishLastName")]
        public string EnglishLastName { get; set; }

        [XmlElement(ElementName="EnglishFirstName")]
        public string EnglishFirstName { get; set; }

        [XmlElement(ElementName="EnglishMiddleName")]
        public string EnglishMiddleName { get; set; }

        [XmlElement(ElementName="BirthDate")]
        public string BirthDate { get; set; }

        [XmlElement(ElementName="Gender")]
        public string Gender { get; set; }

        [XmlElement(ElementName="IssuanceDate")]
        public string IssuanceDate { get; set; }

        [XmlElement(ElementName="ValidityDate")]
        public string ValidityDate { get; set; }
    }

    [XmlRoot(ElementName = "Citizenship")]
    public partial class Citizenship
    {
        [XmlElement(ElementName="CountryName")]
        public string CountryName { get; set; }

        [XmlElement(ElementName="CountryCode")]
        public string CountryCode { get; set; }
    }

    [XmlRoot(ElementName = "DocumentIdentifier")]
    public partial class DocumentIdentifier
    {
        [XmlElement(ElementName="DocumentType")]
        public string DocumentType { get; set; }

        [XmlElement(ElementName="DocumentNumber")]
        public string DocumentNumber { get; set; }
    }

    [XmlRoot(ElementName = "AVVRegistrationAddress")]
    public partial class AvvRegistrationAddress
    {
        [XmlElement(ElementName="LocationCode")]
        public string LocationCode { get; set; }

        [XmlElement(ElementName="Region")]
        public string Region { get; set; }

        [XmlElement(ElementName="Community")]
        public string Community { get; set; }

        [XmlElement(ElementName="Residence")]
        public string Residence { get; set; }

        [XmlElement(ElementName="Street")]
        public string Street { get; set; }

        [XmlElement(ElementName="Building")]        
        public string Building { get; set; }

        [XmlElement(ElementName="BuildingType")]
        public string BuildingType { get; set; }

        [XmlElement(ElementName="Apartment")]
        public string Apartment { get; set; }
    }

    public partial class PDataModel
    {
        public static PDataModel Deserialize(string jsonText)
        {
            JObject json = JObject.Parse(jsonText);
            PDataModel pDataModel = new PDataModel
            {
                Opaque = (string)json["opaque"],
                LastName = (string)json["last_name"],
                FirstName = (string)json["first_name"],
                Ssn = (string)json["SSN"],
                ECivil = (string)json["e_civil"],
                VehicleInfo = (string)json["vehicle_info"],
                DrivingLicense = (string)json["driving_license"],
                CesData = (string)json["ces_data"],
                PassportData = new PassportData
                {
                    PNum = (string)json["passport_data"]["PNum"],
                    SsnIndicator = (bool)json["passport_data"]["SSNIndicator"],
                    CertificateNum = (string)json["passport_data"]["CertificateNum"],
                    Photo = (string)json["passport_data"]["Photo"],
                    IsDead = (bool)json["passport_data"]["IsDead"],
                    AvvDocuments = GetAvvDocuments((string)json["passport_data"]["AVVDocuments"]),
                    AvvRegistrationAddress = JsonConvert.DeserializeObject<AvvRegistrationAddress>((string)json["passport_data"]["AVVRegistrationAddress"])

                }
            };
            return pDataModel;
        }

        private static AvvDocuments GetAvvDocuments(string content)
        {
            AvvDocuments avvDocuments = new AvvDocuments();            
            var token = JToken.Parse(content);

            if (token is JArray)
            {
                avvDocuments.AvvDocument = token.ToObject<List<AvvDocument>>();
            }
            else
            {
                avvDocuments.AvvDocument = new List<AvvDocument>();
                avvDocuments.AvvDocument.Add(token.ToObject<AvvDocument>());
            }

            return avvDocuments;
        }
    }
}
