using AcraUtils.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<PekCallController> _logger;
        static string ValidationMessage;
        private static readonly string logPath = Path.Combine(Directory.GetCurrentDirectory(), "20log.log");
        bool isBackOk = true;

        public PekCallController(PekConfig pekConfig, ILogger<PekCallController> logger)
        {
            _pekConfig = pekConfig;
            _logger = logger;
        }

        // =========================
        // CENTRAL LOG TO PEK BACK (always logs request + response, success or error)
        // =========================
        private async Task SendJournalLog(
            Response response,
            string request,
            bool isTin,
            long userActivityId,
            string source,
            string additionalErrorInfo = null)
        {
            try
            {
                var url = $"{_pekConfig.PekBackUrl}/PekJournal/LogPekResponses";

                // Enrich error message if extra info provided (e.g. exception details)
                if (!string.IsNullOrEmpty(additionalErrorInfo) && response != null)
                {
                    if (string.IsNullOrEmpty(response.errorMessage))
                        response.errorMessage = additionalErrorInfo;
                    else
                        response.errorMessage = $"{response.errorMessage} | {additionalErrorInfo}";
                }

                var pekReq = new AcraUtils.PekReqModel
                {
                    responseModel = response,
                    requestModel = request ?? string.Empty,
                    isTinModel = isTin,
                    userActivityId = userActivityId,
                    SourceID = int.TryParse(source, out var s) ? s : 0
                };

                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };

                var content = new StringContent(JsonConvert.SerializeObject(pekReq), Encoding.UTF8, "application/json");
                var httpResponse = await client.PostAsync(url, content);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    var body = await httpResponse.Content.ReadAsStringAsync();
                    _logger.LogError("PEK Journal log failed. Status={StatusCode}, Body={Body}, Request={Request}",
                        httpResponse.StatusCode, body, request);
                    SafeAppendToFile($"\n{DateTime.Now} LOG FAILED (HTTP {(int)httpResponse.StatusCode}): {body}");
                }
                else
                {
                    _logger.LogInformation("PEK Journal logged successfully. UserActivityId={UserActivityId}, IsTin={IsTin}, Status={Status}",
                        userActivityId, isTin, response?.errorCode ?? "null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendJournalLog failed for request={Request}, userActivityId={UserActivityId}", request, userActivityId);
                SafeAppendToFile($"\n{DateTime.Now} LOG FAILED: {ex}");
            }
        }

        private void SafeAppendToFile(string message)
        {
            try
            {
                System.IO.File.AppendAllText(logPath, message);
            }
            catch
            {
                // swallow – last resort logging must not throw
            }
        }

        // =========================
        // SSN FLOW – logs both success and error paths to PEKJournal
        // =========================
        public async Task<string> GetInfoBySSN(string ssn, string source, long userActivityId)
        {
            var response = new term3();
            Response resultResponse = null;

            if (string.IsNullOrWhiteSpace(ssn))
            {
                resultResponse = new Response
                {
                    errorCode = "7",
                    errorMessage = "SSN is null"
                };
                await SendJournalLog(resultResponse, ssn, false, userActivityId, source);
                return Serialize(resultResponse);
            }

            try
            {
                var client = new AcraServicePortTypeClient();
                var request = new RequestBySsn
                {
                    ssn = ssn,
                    RequestorCode = source
                };

                _logger.LogInformation("SSN REQUEST START. SSN={Ssn}, Source={Source}, UserActivityId={UserActivityId}", ssn, source, userActivityId);
                SafeAppendToFile($"\n{DateTime.Now} SSN REQUEST START");

                response = await client.getAcraInfoBySsnAsync(request);
                resultResponse = response.ResponseBySsn;

                // SUCCESS (or PEK-returned business error) – always log to journal
                await SendJournalLog(resultResponse, ssn, false, userActivityId, source);

                _logger.LogInformation("SSN RESPONSE OK. errorCode={ErrorCode}", resultResponse?.errorCode);
                SafeAppendToFile($"\n{DateTime.Now} SSN RESPONSE OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SSN PEK call failed. SSN={Ssn}, UserActivityId={UserActivityId}", ssn, userActivityId);
                SafeAppendToFile($"\n{DateTime.Now} ERROR: {ex}");

                resultResponse = new Response
                {
                    errorCode = "12",
                    errorMessage = "PEK ERROR"
                };

                // ERROR LOG – always write to PEKJournal
                await SendJournalLog(resultResponse, ssn, false, userActivityId, source, ex.ToString());
            }

            return Serialize(resultResponse);
        }

        // =========================
        // TIN FLOW – logs both success and error paths to PEKJournal
        // =========================
        public async Task<string> GetInfoByTIN(string tin, string source, long userActivityId)
        {
            var response = new term1();
            Response resultResponse = null;

            if (string.IsNullOrWhiteSpace(tin))
            {
                resultResponse = new Response
                {
                    errorCode = "7",
                    errorMessage = "TIN is null"
                };
                await SendJournalLog(resultResponse, tin, true, userActivityId, source);
                return Serialize(resultResponse);
            }

            try
            {
                var client = new AcraServicePortTypeClient();
                var request = new RequestByTin
                {
                    tin = tin,
                    RequestorCode = source
                };

                _logger.LogInformation("TIN REQUEST START. TIN={Tin}, Source={Source}, UserActivityId={UserActivityId}", tin, source, userActivityId);
                SafeAppendToFile($"\n{DateTime.Now} TIN REQUEST START");

                response = await client.getAcraInfoByTinAsync(request);
                resultResponse = response.ResponseByTin;

                // SUCCESS (or PEK-returned business error) – always log to journal
                await SendJournalLog(resultResponse, tin, true, userActivityId, source);

                _logger.LogInformation("TIN RESPONSE OK. errorCode={ErrorCode}", resultResponse?.errorCode);
                SafeAppendToFile($"\n{DateTime.Now} TIN RESPONSE OK");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TIN PEK call failed. TIN={Tin}, UserActivityId={UserActivityId}", tin, userActivityId);
                SafeAppendToFile($"\n{DateTime.Now} ERROR: {ex}");

                resultResponse = new Response
                {
                    errorCode = "12",
                    errorMessage = "PEK ERROR"
                };

                // ERROR LOG – always write to PEKJournal
                await SendJournalLog(resultResponse, tin, true, userActivityId, source, ex.ToString());
            }

            return Serialize(resultResponse);
        }

        // =========================
        // SERIALIZER
        // =========================
        private string Serialize(Response r)
        {
            if (r == null)
            {
                r = new Response { errorCode = "12", errorMessage = "Null response" };
            }
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
                XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = doc.DocumentElement;
                doc.InsertBefore(xmlDeclaration, root);

                XmlElement element1 = doc.CreateElement(string.Empty, "body", string.Empty);
                doc.AppendChild(element1);

                XmlElement element2 = doc.CreateElement(string.Empty, "Message", string.Empty);
                element1.AppendChild(element2);

                XmlElement element3 = doc.CreateElement(string.Empty, "ErrorMessage", string.Empty);
                XmlText text1 = doc.CreateTextNode(st);
                element3.AppendChild(text1);
                element2.AppendChild(element3);

                return doc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StringToXml failed");
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
                _logger.LogWarning("Sending error email: {Message}", txt);
                SafeAppendToFile("Started sendemail");
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(_pekConfig.SMTPClient);
                SafeAppendToFile("Server Created");
                mail.From = new MailAddress(_pekConfig.SendErrorsFromEmail);
                List<string> toMails = _pekConfig.SendErrorsToEmail.Split(',').ToList();
                foreach (var item in toMails)
                {
                    mail.To.Add(item);
                }
                mail.Subject = "Pek Service Error";
                mail.Body = txt;
                SafeAppendToFile("MailText Added");
                SmtpServer.Port = 25;
                // Credentials should come from config / secret store in production
                SmtpServer.Credentials = new System.Net.NetworkCredential("dev.support@acra.am", "Dev$123");
                SmtpServer.EnableSsl = false;

                SmtpServer.Send(mail);
                SafeAppendToFile("mail sent");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SendEmail failed");
                SafeAppendToFile($"Error: {ex}");
            }
        }

        public async Task LogPekActivity(long userActivityId, string message)
        {
            try
            {
                string url = $"{_pekConfig.PekBackUrl}/PekJournal/LogPekActivity?userActivityId={userActivityId}&message={Uri.EscapeDataString(message ?? string.Empty)}";
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(60);
                var result = await client.GetAsync(url);
                if (!result.IsSuccessStatusCode)
                {
                    _logger.LogError("LogPekActivity HTTP failed: {StatusCode}", result.StatusCode);
                }
            }
            catch (Exception ex)
            {
                isBackOk = false;
                _logger.LogError(ex, "LogPekActivity failed, PekBack may be down");
                SendEmail("Pek Back Service is not responding");
                SafeAppendToFile($"Error: {ex}");
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
                return stringwriter.ToString();
            }
        }
    }
}
