using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcraData.Data;
using CheckUpService;
using CheckUpService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CheckUpBackEndService.Controllers
{
    public class CheckUpController : Controller
    {
        private DbContextOptions<Acra3DbContext> _acra3ContextOptions;
        private CheckUpService.CheckUpBackService _CheckUpService;
        private AcraUtils.Logger _logger;

        public CheckUpController(CheckUpService.CheckUpBackService CheckUpService, DbContextOptions<Acra3DbContext> acra3ContextOptions, AcraUtils.Logger logger)
        {
            _acra3ContextOptions = acra3ContextOptions;
            _CheckUpService = CheckUpService;
            _logger = logger;
        }

        [HttpGet]
        //TODO
        public Response RestrictPermissions(string userName)
        {
            try
            {
                return _CheckUpService.RestrictPermissions(userName);
            }
            catch (Exception ex) { _logger.Log.Fatal("PackUpController.RestrictPermissions:", ex); return _CheckUpService.GetErrorResponse(ex.Message); }
        }

        [HttpGet]
        public Response GetSource(string userName)
        {
            try
            {
                return _CheckUpService.GetSource(userName);
            }
            catch (Exception ex) { _logger.Log.Fatal("PackUpController.GetSource:", ex); return _CheckUpService.GetErrorResponse(ex.Message); }
        }

        [HttpGet]
        public Response GetIsMemberOrg(string userName, string source)
        {
            try
            {
                return _CheckUpService.GetIsMemberOrg(userName, source);
            }
            catch (Exception ex) { _logger.Log.Fatal("PackUpController.GetIsMemberOrg:", ex); return _CheckUpService.GetErrorResponse(ex.Message); }
        }

        [HttpGet]
        public Response RegisterReceivedPackage(string sessionID, string userName, string sourceName, string path, string fileName,string thumbprint)
        {
            try
            {
                return _CheckUpService.RegisterReceivedPackageInfo(sessionID, userName, sourceName, path, fileName,thumbprint);
            }
            catch (Exception ex) { _logger.Log.Fatal("PackUpController.RegisterReceivedPackage:", ex); return _CheckUpService.GetErrorResponse(ex.Message); }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}