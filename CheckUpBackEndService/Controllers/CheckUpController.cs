using System;
using System.Threading.Tasks;
using AcraData.Data;
using CheckUpService;
using CheckUpService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CheckUpBackEndService;

namespace CheckUpBackEndService.Controllers
{
    public class CheckUpController : Controller
    {
        private readonly DbContextOptions<Acra3DbContext> _acra3ContextOptions;
        private readonly CheckUpService.CheckUpBackService _CheckUpService;
        private readonly AcraUtils.Logger _logger;
        private readonly ILogger<CheckUpController> _msLogger;
        private readonly ElasticJournalService _elastic;

        public CheckUpController(
            CheckUpService.CheckUpBackService CheckUpService,
            DbContextOptions<Acra3DbContext> acra3ContextOptions,
            AcraUtils.Logger logger,
            ILogger<CheckUpController> msLogger,
            ElasticJournalService elastic)
        {
            _acra3ContextOptions = acra3ContextOptions;
            _CheckUpService = CheckUpService;
            _logger = logger;
            _msLogger = msLogger;
            _elastic = elastic;
        }

        /// <summary>
        /// Central helper: always writes to Elastic journal (success + error paths).
        /// Fire-and-forget so journal latency never blocks the client.
        /// </summary>
        private void LogToJournal(CheckUpJournalDocument doc)
        {
            try
            {
                _ = _elastic.LogAsync(doc);
            }
            catch (Exception ex)
            {
                // Should never happen because LogAsync already swallows, but extra safety
                _msLogger.LogError(ex, "Failed to schedule CheckUp journal log. EventType={EventType}", doc?.EventType);
            }
        }

        [HttpGet]
        public Response RestrictPermissions(string userName)
        {
            Response result;
            try
            {
                result = _CheckUpService.RestrictPermissions(userName);
            }
            catch (Exception ex)
            {
                _logger.Log.Fatal("CheckUpController.RestrictPermissions:", ex);
                _msLogger.LogError(ex, "RestrictPermissions failed for userName={UserName}", userName);
                result = _CheckUpService.GetErrorResponse(ex.Message);
            }

            // Always log to journal (success or error)
            LogToJournal(new CheckUpJournalDocument
            {
                EventType  = "RestrictPermissions",
                UserName   = userName,
                ErrorCode  = result?.ErrorCode ?? -1,
                ErrorDesc  = result?.ErrorDesc
            });

            return result;
        }

        [HttpGet]
        public Response GetSource(string userName)
        {
            Response result;
            try
            {
                result = _CheckUpService.GetSource(userName);
            }
            catch (Exception ex)
            {
                _logger.Log.Fatal("CheckUpController.GetSource:", ex);
                _msLogger.LogError(ex, "GetSource failed for userName={UserName}", userName);
                result = _CheckUpService.GetErrorResponse(ex.Message);
            }

            LogToJournal(new CheckUpJournalDocument
            {
                EventType = "GetSource",
                UserName  = userName,
                ErrorCode = result?.ErrorCode ?? -1,
                ErrorDesc = result?.ErrorDesc
            });

            return result;
        }

        [HttpGet]
        public Response GetIsMemberOrg(string userName, string source)
        {
            Response result;
            try
            {
                result = _CheckUpService.GetIsMemberOrg(userName, source);
            }
            catch (Exception ex)
            {
                _logger.Log.Fatal("CheckUpController.GetIsMemberOrg:", ex);
                _msLogger.LogError(ex, "GetIsMemberOrg failed for userName={UserName}, source={Source}", userName, source);
                result = _CheckUpService.GetErrorResponse(ex.Message);
            }

            LogToJournal(new CheckUpJournalDocument
            {
                EventType  = "GetIsMemberOrg",
                UserName   = userName,
                SourceName = source,
                ErrorCode  = result?.ErrorCode ?? -1,
                ErrorDesc  = result?.ErrorDesc
            });

            return result;
        }

        [HttpGet]
        public Response RegisterReceivedPackage(
            string sessionID, string userName, string sourceName,
            string path, string fileName, string thumbprint)
        {
            Response result;
            try
            {
                result = _CheckUpService.RegisterReceivedPackageInfo(
                    sessionID, userName, sourceName, path, fileName, thumbprint);
            }
            catch (Exception ex)
            {
                _logger.Log.Fatal("CheckUpController.RegisterReceivedPackage:", ex);
                _msLogger.LogError(ex, "RegisterReceivedPackage failed. UserName={UserName}, FileName={FileName}", userName, fileName);
                result = _CheckUpService.GetErrorResponse(ex.Message);
            }

            LogToJournal(new CheckUpJournalDocument
            {
                EventType  = "RegisterPackage",
                UserName   = userName,
                SourceName = sourceName,
                SessionId  = sessionID,
                FileName   = fileName,
                Thumbprint = thumbprint,
                ErrorCode  = result?.ErrorCode ?? -1,
                ErrorDesc  = result?.ErrorDesc
            });

            return result;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
