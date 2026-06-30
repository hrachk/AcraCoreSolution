using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AcraData.Data;
using AcraUtils;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CheckUpService.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using AcraUtils.Configuration;
using Microsoft.Extensions.Options;
using AcraData.Models.Acra3;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using OfficeOpenXml;

namespace CheckUpService
{
    public class CheckUpBackService
    {
        private Logger _logger;
        private DbContextOptions<Acra3DbContext> _acra3DbContextOptions;


        public CheckUpBackService(DbContextOptions<Acra3DbContext> acra3DbContextOptions, Logger logger)
        {
            _acra3DbContextOptions = acra3DbContextOptions;
            _logger = logger;
        }

        public Response GetErrorResponse(string errorDesc)
        {
            return new Response() { ErrorCode = 206, ResponseTime = DateTime.Now.Ticks, ErrorDesc = errorDesc };
        }

        public Response RegisterReceivedPackageInfo(string sessionID, string userName, string sourceName, string path, string fileName, string thumbprint)
        {
            Response response = new Response();
            using (var context = new Acra3DbContext(_acra3DbContextOptions))
            {

                using (var tx = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Получаем SourceId безопасно
                        var packageSrc = System.Linq.Queryable
                            .Where(context.Sources, s => s.ShortName == sourceName)
                            .Select(s => s.SourceId)
                            .FirstOrDefault();

                        // Переназначаем при совпадении
                        if (packageSrc == 68 || packageSrc == 73 || packageSrc == 60 || packageSrc == 59 || packageSrc == 62 || packageSrc == 61)
                        {
                            packageSrc = 18;
                        }

                        string DBThumbprint = string.Empty;

                        // Ищем Thumbprint напрямую через Any + FirstOrDefault
                        var thumbprintEntry = context.DicSynonyms
                            .FirstOrDefault(d => d.SourceID == packageSrc && d.Type == "Thumbprint");

                        if (thumbprintEntry != null)
                        {
                            DBThumbprint = thumbprintEntry.BankValue;
                        }


                        if (DBThumbprint == string.Empty || thumbprint.Replace(" ", "") != DBThumbprint.Replace(" ", ""))
                        {

                            using (StreamWriter sw = System.IO.File.AppendText($"thumbprint_{DateTime.Now.ToString("yyyy-MM-dd")}.log"))
                            {
                                DateTime localDate = DateTime.Now;

                                sw.WriteLine($"[{System.Text.Json.JsonSerializer.Serialize(localDate)}] {packageSrc}, Actual TP: {thumbprint.Replace(" ", "")} | TP in databse: {DBThumbprint.Replace(" ", "")} ;");
                            }

                            response.ErrorCode = 501;
                            response.ErrorDesc = "Ֆայլը ստորագրվել է ոչ թույլատրելի սերտիֆիկատով";
                            return response;
                        }
                        //if (CheckFileCertificate(Path.Combine(path, fileName), sourceName, userName))
                        //{
                        var userInfo = Queryable.Where(context.UserInfos, u => u.UserLogin == userName).SingleOrDefault();
                        AcraData.Models.Acra3.ReceivedPacket receivedPacket = new AcraData.Models.Acra3.ReceivedPacket();
                        AcraData.Models.Acra3.PackageFile packageFile = new AcraData.Models.Acra3.PackageFile();
                        if (string.IsNullOrEmpty(sessionID))
                        {
                            response.ErrorCode = 202;
                            response.ErrorDesc = "Սխալ հարցում"; /* Wrong SessionID */
                        }
                        else
                        {
                            // Insert into ReceivedPacket and PackageFile
                              var packageSource =  System.Linq.Queryable.Where(context.Sources,s => s.ShortName == sourceName).FirstOrDefault();
                        
                            if (packageSource != null)
                            {
                                receivedPacket.UserID = userInfo.UserId;
                                receivedPacket.SourceID = packageSource.SourceId;
                                receivedPacket.IncomingDate = DateTime.Now;

                                int _index = fileName.LastIndexOf('_');
                                int _extIndex = fileName.LastIndexOf('.');
                                receivedPacket.ExternalPackageID = (_index != -1 && _extIndex != -1) ? fileName.Substring(_index + 1, _extIndex - _index - 1) : fileName;
                                receivedPacket.FileCount = 1;
                                receivedPacket.PackageStatus = 1;
                                receivedPacket.ConvertStatus = 0;
                                receivedPacket.StatusModifyDate = DateTime.Now;

                                context.ReceivedPackets.Add(receivedPacket);
                                context.SaveChanges();

                                packageFile.ReceivedPackageID = receivedPacket.ReceivedPackageID;
                                packageFile.SourceID = receivedPacket.SourceID;
                                packageFile.ExternalPackageID = receivedPacket.ExternalPackageID;
                                packageFile.FileName = fileName;
                                packageFile.FileCount = 1;
                                packageFile.FileNum = 1;
                                packageFile.FileStatus = 1;
                                //packageFile.ReceivedDate = DateTime.Now;

                                context.PackageFiles.Add(packageFile);
                                context.SaveChanges();

                                tx.Commit();
                                _logger.Log.Info($"File: {fileName} RegisterReceivedPackageInfo success");
                                response.ErrorCode = 200;
                                response.ErrorDesc = "Գործողության բարեհաջող ավարտ"; /*Success*/
                            }
                            else
                            {
                                response.ErrorCode = 203;
                                response.ErrorDesc = "Բառարանում աղբյուրի անվանումը բացակայում է"; /* Wrong SourceName */
                                _logger.Log.Error($"File: {fileName} Source name incorrect");
                            }
                        }
                        
                    }
                    catch (MemberAccessException ex)
                    {
                        response.ErrorCode = 208;
                        response.ErrorDesc += $"Օգտագործողի արտոնության անհամապատասխանություն Message: {ex.Message} Inner: {ex.InnerException}"; /* File Download Failed */
                        _logger.Log.Fatal($"File: {fileName} RegisterReceivedPackageInfo failed Error: {ex.Message}");
                        tx.Rollback();
                    }
                    catch (ArgumentNullException ex)
                    {
                        response.ErrorCode = 209;
                        response.ErrorDesc += $"Հավաստագրի վավերացման սխալ Message: {ex.Message} Inner: {ex.InnerException}"; /* File Download Failed */
                        _logger.Log.Fatal($"File: {fileName} RegisterReceivedPackageInfo failed Error: {ex.Message}");
                        tx.Rollback();
                    }
                    catch (CryptographicException ex)
                    {
                        response.ErrorCode = 206;
                        response.ErrorDesc += $"Ֆայլի հավաստագրի ընթերցման ձախողում {fileName} Message: {ex.Message} Inner: {ex.InnerException}"; /* File Download Failed */
                        _logger.Log.Fatal($"File: {fileName} RegisterReceivedPackageInfo failed Error: {ex.Message}");
                        tx.Rollback();
                    }
                    catch (Exception ex)
                    {
                        response.ErrorCode = 205;
                        response.ErrorDesc += $"Ֆայլի վերբեռնման արձանագրության ձախողում {fileName} Message: {ex.Message} Inner: {ex.InnerException}"; /* File Download Failed */
                        _logger.Log.Fatal($"File: {fileName} RegisterReceivedPackageInfo failed Error: {ex.Message}");
                        tx.Rollback();
                    }
                }
            }
            return response;
        }


        public Response RestrictPermissions(string userName)
        {
            Response identityResponse = GetErrorResponse(string.Empty);
            try
            {
                identityResponse.ResponseID = "-1";

                using (var DB = new Acra3DbContext(_acra3DbContextOptions))
                {

                    if (!string.IsNullOrEmpty(userName))
                    {
                        var userInfo = Queryable.Where(DB.UserInfos,u => u.UserLogin == userName).SingleOrDefault();
                        /*Check Password expired Days*/
                        // if (userInfo.Where(u => u.UserPassCreationDate == null || DbFunctions.DiffDays(u.UserPassCreationDate, DateTime.Now) < 90).Count() == 0)
                        if (userInfo.UserPassCreationDate == null || (DateTime.Now - userInfo.UserPassCreationDate).Value.Days >= 90)
                        {
                            identityResponse.ErrorCode = 103;
                            identityResponse.ErrorDesc = "Գաղտնաբառի ժամկետը լրացել է"; /*Password expired*/

                        }
                        /*Check User Status*/
                        else if (userInfo.Status == 2)
                        {
                            identityResponse.ErrorCode = 104;
                            identityResponse.ErrorDesc = "Մուտքն արգելափակված է"; /*Disabled By Admin*/

                        }
                        else if (!DB.UserInterfacePrivileges.Any(p => p.UserID == userInfo.UserId && p.InterfeaceID == 5))
                        {
                            identityResponse.ErrorCode = 105;
                            identityResponse.ErrorDesc = "Փաթեթ վերբեռնելու իրավասություն առկա չէ"; /*Disabled By BankAdmin*/

                        }
                        else
                        {
                            identityResponse.ErrorCode = 200;
                            identityResponse.ErrorDesc = "Գործողության բարեհաջող ավարտ"; /*Success*/
                        }

                        LoginLog loginlog = new LoginLog();

                        try
                        { loginlog.UserLogin = userInfo.UserLogin; }
                        catch { }
                        try
                        {
                            loginlog.UserId = userInfo.UserId;
                        }
                        catch { }
                        try
                        {
                            loginlog.SourceId = userInfo.ClientId;
                        }
                        catch { }

                        try
                        {
                            loginlog.LoginDateTime = DateTime.Now;
                        }
                        catch { }
                        loginlog.SessionId = string.Empty;
                        try
                        {
                            loginlog.SessionId = Guid.NewGuid().ToString();
                        }
                        catch { }

                        DB.LoginLogs.Add(loginlog);
                        DB.SaveChanges();
                        identityResponse.ResponseID = loginlog.SessionId;
                    }
                    else
                    {
                        identityResponse.ErrorCode = 101;
                        identityResponse.ErrorDesc = "Մուտքային տվյալների սխալ"; /* Wrong currUserID */
                        identityResponse.ResponseID = string.Empty;
                    }
                }


                _logger.Log.Info("PackUpController.RestrictPermissions Completed");
            }
            catch (Exception ex) { _logger.Log.Fatal("PackUpController.RestrictPermissions:", ex); identityResponse.ErrorDesc = ex.Message; }

            identityResponse.ResponseTime = DateTime.Now.Ticks;
            return identityResponse;

        }

        public Response GetSource(string userName)
        {
            Response response = GetErrorResponse(string.Empty);
            try
            {
                response.ResponseID = Guid.NewGuid().ToString();

                using (var DB = new Acra3DbContext(_acra3DbContextOptions))
                {

                    if (!string.IsNullOrEmpty(userName))
                    {
                        var userClientID = Queryable.Where(DB.UserInfos,u => u.UserLogin == userName).Select(p => p.ClientId).SingleOrDefault();
                        var source = Queryable.Where(DB.Sources,p => p.SourceId == userClientID).Select(p => p.ShortName).FirstOrDefault();
                        /*Check Password expired Days*/
                        // if (userInfo.Where(u => u.UserPassCreationDate == null || DbFunctions.DiffDays(u.UserPassCreationDate, DateTime.Now) < 90).Count() == 0)
                        if (string.IsNullOrEmpty(source))
                        {
                            response.ErrorCode = 301;
                            response.ErrorDesc = "Անհայտ աղբյուր"; /*Password expired*/

                        }
                        else
                        {
                            response.ResponseMessage = source;
                            response.ErrorCode = 200;
                            response.ErrorDesc = "Գործողության բարեհաջող ավարտ"; /*Success*/
                        }
                    }
                }

                _logger.Log.Info("PackUpController.GetSource Completed");
            }
            catch (Exception ex) { _logger.Log.Fatal("PackUpController.GetSource:", ex); response.ErrorDesc = ex.Message; }

            response.ResponseTime = DateTime.Now.Ticks;
            return response;

        }

        public Response GetIsMemberOrg(string userName, string source)
        {
            Response response = GetErrorResponse(string.Empty);
            try
            {
                response.ResponseID = Guid.NewGuid().ToString();

                using (var DB = new Acra3DbContext(_acra3DbContextOptions))
                {

                    if (!string.IsNullOrEmpty(userName))
                    {
                        var userClientID = Queryable.Where(DB.UserInfos,u => u.UserLogin == userName).Select(p => p.ClientId).SingleOrDefault();
                        /*Check Password expired Days*/
                        // if (userInfo.Where(u => u.UserPassCreationDate == null || DbFunctions.DiffDays(u.UserPassCreationDate, DateTime.Now) < 90).Count() == 0)
                        if (string.IsNullOrEmpty(source))
                        {
                            response.ErrorCode = 301;
                            response.ErrorDesc = "Անհայտ աղբյուր"; /*Password expired*/

                        }
                        else
                        {
                            response.ResponseMessage = IsMemberOrg(source);
                            response.ErrorCode = 200;
                            response.ErrorDesc = "Գործողության բարեհաջող ավարտ"; /*Success*/
                        }
                    }
                }

                _logger.Log.Info("PackUpController.GetSource Completed");
            }
            catch (Exception ex) { _logger.Log.Fatal("PackUpController.GetSource:", ex); response.ErrorDesc = ex.Message; }

            response.ResponseTime = DateTime.Now.Ticks;
            return response;

        }

        private bool IsMemberOrg(string source)
        {
            bool result;
            switch (source)
            {
                case "SNA":
                case "VCL":
                case "BLE":
                case "ORG":
                    result = false;
                    break;
                default:
                    result = true;
                    break;
            }

            return result;
        }

        public static bool IsSigned(string filepath)
        {
            var cert = System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromSignedFile(filepath);

            //using (X509Certificate2 cert = new X509Certificate2(filepath))
            //{
            //    X509Chain chain = new X509Chain();
            //    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreNotTimeValid;

            //    bool validChain = chain.Build(cert);

            //    if (!validChain)
            //    {
            //        // Whatever you want to do about that.

            //        foreach (var status in chain.ChainStatus)
            //        {
            //            // In reality you can == this, since X509Chain.ChainStatus builds
            //            // an object per flag, but since it's [Flags] let's play it safe.
            //            if ((status.Status & X509ChainStatusFlags.PartialChain) != 0)
            //            {
            //                // Incomplete chain.
            //            }
            //        }
            //    }

            //    X509Certificate2Collection chainCerts = new X509Certificate2Collection();

            //    foreach (var element in chain.ChainElements)
            //    {
            //        chainCerts.Add(element.Certificate);
            //    }

            //    // now chainCerts has the whole chain in order.
            //}
            return true;
        }


        public bool CheckFileCertificate(string filename, string source, string userName)
        {
            bool result = false;// IsSigned(filename);
            try
            {
                var package = new ExcelPackage(new FileInfo(filename));
                var signature = package.Workbook.VbaProject.Signature;


                X509Certificate2 x509 = signature.Certificate;

                using (var context = new Acra3DbContext(_acra3DbContextOptions))
                {
                    var userClientID = Queryable.Where(context.UserInfos,u => u.UserLogin == userName).Select(p => p.ClientId).SingleOrDefault();
                    var sourceID = Queryable.Where(context.Sources,u => u.ShortName == source).Select(p => p.SourceId).SingleOrDefault();
                    var thumbprints = Queryable.Where(context.DicSynonyms,p => p.Type.Equals("Thumbprint") && p.SourceID == sourceID).SingleOrDefault();

                    if (thumbprints != null)
                    {
                        if (userClientID == sourceID)
                        {
                            if (x509.Thumbprint.Equals(thumbprints.BankValue))
                                result = true;
                            else
                                result = false;
                        }
                        else
                        {
                            if (userClientID == 18)
                            {
                                if (!IsMemberOrg(source) || userName.Equals("Service"))
                                {
                                    if (x509.Thumbprint.Equals(thumbprints.AcraValue))
                                        result = true;
                                    else
                                        result = false;
                                }
                                else
                                    throw new MemberAccessException("User privilege problem");
                            }
                            else
                                throw new MemberAccessException("User privilege problem");
                        }
                    }
                    else
                        throw new ArgumentNullException("Thumbprint not found");
                }

                return result;
                // throw new CryptographicException("Couldn't parse the certificate.");                                   
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Couldn't parse the certificate.");
            }
        }
    }
}
