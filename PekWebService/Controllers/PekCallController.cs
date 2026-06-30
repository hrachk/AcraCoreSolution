using AcraUtils.Configuration;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PEK_ServiceReference;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace PekWebService.Controllers
{
    public class PekCallController : Controller
    {
        private readonly PekConfig _pekConfig;
        static string ValidationMessage;
        //TODO Directory.GetCurrentDirectory() + @"\log.log"
        private static string logPath = Path.Combine(Directory.GetCurrentDirectory(), "20log.log");
        bool isBackOk = true;

        public PekCallController(PekConfig pekConfig )
        {
            _pekConfig = pekConfig;
             
        }
         
        // =========================
        // CENTRAL LOG TO PEK BACK
        // =========================
        private async Task SendJournalLog(
            Response response,
            string request,
            bool isTin,
            long userActivityId,
            string source)
        {
            try
            {
                var url = $"{_pekConfig.PekBackUrl}/PekJournal/LogPekResponses";

                var pekReq = new AcraUtils.PekReqModel
                {
                    responseModel = response,
                    requestModel = request,
                    isTinModel = isTin,
                    userActivityId = userActivityId,
                    SourceID = int.TryParse(source, out var s) ? s : 0
                };

                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                await client.PostAsync(
                    url,
                    new StringContent(JsonConvert.SerializeObject(pekReq), Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logPath, $"\n{DateTime.Now} LOG FAILED: {ex}");
            }
        }

        // =========================
        // SSN FLOW
        // =========================
        public async Task<string> GetInfoBySSN(string ssn, string source, long userActivityId)
        {
            var response = new term3();

            if (string.IsNullOrWhiteSpace(ssn))
            {
                response.ResponseBySsn = new Response
                {
                    errorCode = "7",
                    errorMessage = "SSN is null"
                };

                await SendJournalLog(response.ResponseBySsn, ssn, false, userActivityId, source);
                return Serialize(response.ResponseBySsn);
            }

            try
            {
                var client = new AcraServicePortTypeClient();
                var request = new RequestBySsn
                {
                    ssn = ssn,
                    RequestorCode = source
                };

                System.IO.File.AppendAllText(logPath, $"\n{DateTime.Now} SSN REQUEST START");

                response = await client.getAcraInfoBySsnAsync(request);

                // =========================
                // SUCCESS LOG
                // =========================
                await SendJournalLog(
                    response.ResponseBySsn,
                    ssn,
                    false,
                    userActivityId,
                    source);

                System.IO.File.AppendAllText(logPath, $"\n{DateTime.Now} SSN RESPONSE OK");
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logPath, $"\n{DateTime.Now} ERROR: {ex}");

                response.ResponseBySsn = new Response
                {
                    errorCode = "12",
                    errorMessage = "PEK ERROR"
                };

                // =========================
                // ERROR LOG (IMPORTANT)
                // =========================
                await SendJournalLog(
                    response.ResponseBySsn,
                    ssn,
                    false,
                    userActivityId,
                    source);
            }

            return Serialize(response.ResponseBySsn);
        }

        // =========================
        // SERIALIZER
        // =========================
        private string Serialize(Response r)
        {
            using var sw = new StringWriter();
            var ser = new XmlSerializer(typeof(Response));
            ser.Serialize(sw, r);
            return sw.ToString();
        }

        public XmlDocument StringToXml(string st)
        {
            try
            {

                XmlDocument doc = new XmlDocument();

                //(1) the xml declaration is recommended, but not mandatory
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                //(2) string.Empty makes cleaner code
                XmlElement element1 = doc.CreateElement(string.Empty, "body", string.Empty);
                doc.AppendChild(element1);

                XmlElement element2 = doc.CreateElement(string.Empty, "Message", string.Empty);
                element1.AppendChild(element2);

                XmlElement element3 = doc.CreateElement(string.Empty, "ErrorMessage", string.Empty);
                XmlText text1 = doc.CreateTextNode(st);
                element3.AppendChild(text1);
                element2.AppendChild(element3);

                /*XmlElement element4 = doc.CreateElement(string.Empty, "level2", string.Empty);
                XmlText text2 = doc.CreateTextNode("other text");
                element4.AppendChild(text2);
                element2.AppendChild(element4);*/

                return doc;
            }
            catch (Exception ex)
            {
                //SendEmail(ex.Message);
                throw;
            }
        }
        public void ValidateXml(string validatbleXml)
        {
            XmlReaderSettings booksSettings = new XmlReaderSettings();
            booksSettings.Schemas.Add("http://www.taxservice.am/tp3/acra/definitions", "acra.xsd");
            booksSettings.ValidationType = ValidationType.Schema;
            booksSettings.ValidationEventHandler += new ValidationEventHandler(booksSettingsValidationEventHandler);
            System.IO.File.WriteAllText("foo.xml", validatbleXml);
            //XmlReader books = XmlReader.Create("foo.xml",booksSettings);
            XmlReader books = XmlReader.Create(new StringReader(System.IO.File.ReadAllText("foo.xml")), booksSettings);
            while (books.Read()) { }
        }
        static void booksSettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                ValidationMessage = $"Warning: {e.Message}";
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                ValidationMessage = $"Error: {e.Message}";
            }
        }
        public void SendEmail(string txt)
        {
            try
            {
                System.IO.File.AppendAllText(logPath, "Started sendemail");
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(_pekConfig.SMTPClient);
                System.IO.File.AppendAllText(logPath, "Server Created");
                mail.From = new MailAddress(_pekConfig.SendErrorsFromEmail);
                List<string> toMails = _pekConfig.SendErrorsToEmail.Split(',').ToList();
                foreach (var item in toMails)
                {
                    mail.To.Add(item);
                }
                mail.Subject = "Pek Service Error";
                mail.Body = txt;
                System.IO.File.AppendAllText(logPath, "MailText Added");
                SmtpServer.Port = 25;
                SmtpServer.Credentials = new System.Net.NetworkCredential("dev.support@acra.am", "Dev$123");
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(mail);
                System.IO.File.AppendAllText(logPath, "mail sent");
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(logPath, $"Error: {ex}");
            }
        }
        public async Task LogPekActivity(long userActivityId, string message)
        {
            try
            {
                string url = $"{_pekConfig.PekBackUrl}/PekJournal/LogPekActivity?userActivityId={userActivityId}&message={message.Replace(' ', '_')}";
                var client = new HttpClient();
                client.Timeout = new TimeSpan(0, 1, 0);
                var result = await client.GetAsync(url);
            }
            catch (Exception ex)
            {
                isBackOk = false;
                SendEmail("Pek Back Service is not responding");
                System.IO.File.AppendAllText(logPath, $"Error: {ex}");
            }


        }
        public string CheckBack()
        {
            isBackOk = true;
            term1 response = new term1();
            response.ResponseByTin = new Response();
            response.ResponseByTin.errorCode = "11";
            response.ResponseByTin.errorMessage = "PekBack is not Responding";
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(response.ResponseByTin.GetType());
                serializer.Serialize(stringwriter, response.ResponseByTin);
                //ValidateXml(stringwriter.ToString().Replace("<OutstandingTaxDebt>0", "<OutstandingTaxDebt>a"));
                return stringwriter.ToString();
            }
        }
    }
}