using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcraData.Data;
using CheckUpService;
using CheckUpService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheckUpBackEndService;

namespace CheckUpBackEndService.Controllers
{
    public class CheckUpController : Controller
    {
        private DbContextOptions<Acra3DbContext> _acra3ContextOptions;
        private CheckUpService.CheckUpBackService _CheckUpService;
        private AcraUtils.Logger _logger;
        private readonly ElasticJournalService _elastic;

        public CheckUpController(
            CheckUpService.CheckUpBackService CheckUpService,
            DbContextOptions<Acra3DbContext> acra3ContextOptions,
            AcraUtils.Logger logger,
            ElasticJournalService elastic)
        {
            _acra3ContextOptions = acra3ContextOptions;
            _CheckUpService = CheckUpService;
            _logger = logger;
            _elastic = elastic;
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
                result = _CheckUpService.GetErrorResponse(ex.Message);
            }

            _ = _elastic.LogAsync(new CheckUpJournalDocument
            {
                EventType  = "RestrictPermissions",
                UserName   = userName,
                ErrorCode  = result.ErrorCode,
                ErrorDesc  = result.ErrorDesc
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
                result = _CheckUpService.GetErrorResponse(ex.Message);
            }

            _ = _elastic.LogAsync(new CheckUpJournalDocument
            {
                EventType = "GetSource",
                UserName  = userName,
                ErrorCode = result.ErrorCode,
                ErrorDesc = result.ErrorDesc
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
                result = _CheckUpService.GetErrorResponse(ex.Message);
            }

            _ = _elastic.LogAsync(new CheckUpJournalDocument
            {
                EventType  = "GetIsMemberOrg",
                UserName   = userName,
                SourceName = source,
                ErrorCode  = result.ErrorCode,
                ErrorDesc  = result.ErrorDesc
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
                result = _CheckUpService.GetErrorResponse(ex.Message);
            }

            _ = _elastic.LogAsync(new CheckUpJournalDocument
            {
                EventType  = "RegisterPackage",
                UserName   = userName,
                SourceName = sourceName,
                SessionId  = sessionID,
                FileName   = fileName,
                Thumbprint = thumbprint,
                ErrorCode  = result.ErrorCode,
                ErrorDesc  = result.ErrorDesc
            });

            return result;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
