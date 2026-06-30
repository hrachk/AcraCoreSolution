using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcraData.Data;
using AcraData.Models.Acra3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheckUpService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using AcraUtils.Configuration;
using System.Net.Http;
using Newtonsoft.Json;

namespace CheckUpBackService.Controllers
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

        public Response RestrictPermissions(string userName)
        {
            try
            {
                return _CheckUpService.RestrictPermissions(userName);
            }
            catch (Exception ex) { _logger.Log.Fatal("PackUpController.RestrictPermissions:", ex); return _CheckUpService.GetErrorResponse(ex.Message); }        
        }

        public Response RegisterReceivedPackage(string userName, string fileName, string sessionID, string sourceName)
        {
            try
            {
                return _CheckUpService.RegisterReceivedPackageInfo(userName, fileName, sessionID, sourceName);
            }                         
            catch (Exception ex) { _logger.Log.Fatal("PackUpController.RegisterReceivedPackage:", ex); return _CheckUpService.GetErrorResponse(ex.Message); }
        }
    }
}