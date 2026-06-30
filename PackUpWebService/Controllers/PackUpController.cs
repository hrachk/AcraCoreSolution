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
using CheckUpWebService.Models;
using Microsoft.AspNetCore.Hosting;
using CheckUpWebService.Infrastructure;
using Microsoft.Extensions.Options;
using AcraUtils.Configuration;
using System.Net.Http;
using Newtonsoft.Json;

namespace CheckUpWebService.Controllers
{
    //[Route("PackUp/[controller]")]
    public class PackUpController : Controller
    {
       // private readonly IHostingEnvironment _env;
        private int? sourceId = null;
        private int userId = -1;       
        private AcraUtils.Logger _logger;        
        private CheckUpService.CheckUpService _CheckUpService;
        private PackUpConfig _configuration;
        private AcraUtils.Cryptor cryptor = new AcraUtils.Cryptor();

        public PackUpController(CheckUpService.CheckUpService CheckUpService, AcraUtils.Logger logger, IOptions<PackUpConfig> configuration)        
        {                      
            _CheckUpService = CheckUpService;
            _configuration = configuration.Value;
            _logger = logger;
        }
       
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<Response> Post(string sourceName, string username, string sessionID, string thumbprint, string version)
        {
            sourceName = cryptor.DecryptDES(sourceName);
            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} sourcename = {sourceName}" + Environment.NewLine);
            username = cryptor.DecryptDES(username);
            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} username = {username}" + Environment.NewLine);
            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} sessionID before decrypt = {sessionID}" + Environment.NewLine);
            sessionID = sessionID.Replace(" ", "+");
            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} sessionID before decrypt+ = {sessionID}" + Environment.NewLine);
            sessionID = RemoveWhitespace(cryptor.DecryptDES(sessionID));
            //sessionID = RemoveWhitespace(cryptor.DecryptDES(sessionID));
            //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} sessionID = {sessionID}" + Environment.NewLine);
            thumbprint = thumbprint.Replace(" ", "+");
            thumbprint = cryptor.DecryptDES(thumbprint);

            version = version.Replace(" ", "+");
            version = cryptor.DecryptDES(version);
            Response response = new Response();
            if (version != _configuration.VersionControl)
            {
                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} verion = {version}" + Environment.NewLine);
                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} verion = {_configuration.VersionControl}" + Environment.NewLine);
                response.ResponseMessage = "Առկա է տվյալ ծրագրի նոր տարբերակ, անհրաժեշտ է թարմացնել";
                response.ErrorCode = 1025;
                return response;
            }
            if (_configuration.Switch == false)
            {
                response.ResponseMessage = $"Տվյալ պահին վերբեռնումը դադարեցված է, խնդրում եմ կրկին փորձել {_configuration.TimeOfReopening}-ից հետո";
                response.ErrorCode = 1024;
                return response;
            }
            try
            {               
                //1. Upload File
                response = await _CheckUpService.Upload(Request.Form.Files[0]);
                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} response = {response}" + Environment.NewLine);
                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} response.ToString() = {response.ToString()}" + Environment.NewLine);
                //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} response.ErrorCode = {response.ErrorCode}" + Environment.NewLine);
                if (response.ErrorCode == 200)
                {
                    //2. Register Received File
                    HttpClient httpClient = new HttpClient();
                    var parameters = $"?sessionID={sessionID}&userName={username}&sourceName={sourceName}&path={_configuration.Destination[0]}&fileName={Request.Form.Files[0].FileName}&thumbprint={thumbprint}";

                    var responseMsg = httpClient.GetAsync($"{_configuration.CheckUpBackEndURL}/CheckUp/RegisterReceivedPackage{parameters}").Result;

                    if (responseMsg.IsSuccessStatusCode)
                    {
                        response = JsonConvert.DeserializeObject<Response>(responseMsg.Content.ReadAsStringAsync().Result);
                        //System.IO.File.AppendAllText("C:/Logs/log.txt", $"{DateTime.Now} response after JsonConvert = {response}" + Environment.NewLine);
                    }
                }
                                      
            }
            catch (Exception ex)
            {
                _logger.Log.Error("PackUpController.GetReport Completed", ex);
                return _CheckUpService.GetErrorResponse(ex.Message);                
            }
            _logger.Log.Info($"Post Complete with Response: {response.ErrorCode}: {response.ErrorDesc}");
            return response;
        }


        //[Route("GetSource")]
        [HttpGet]
        //TODO
        public async Task<Response> GetSource(string username)
        {
            username = cryptor.DecryptDES(username);
            Response response = new Response();
            try
            {
                HttpClient httpClient = new HttpClient();
                var parameters = $"?userName={username}";
                HttpResponseMessage responseMsg = httpClient.GetAsync($"{_configuration.CheckUpBackEndURL}/CheckUp/RestrictPermissions{parameters}").Result;
                if (responseMsg.IsSuccessStatusCode)
                {
                    response = JsonConvert.DeserializeObject<Response>(responseMsg.Content.ReadAsStringAsync().Result);
                    if (response.ErrorCode == 200)
                    {
                        string SessionID = response.ResponseID;
                        //1. Get Source
                        httpClient = new HttpClient();

                        responseMsg = httpClient.GetAsync($"{_configuration.CheckUpBackEndURL}/CheckUp/GetSource{parameters}").Result;
                        if (responseMsg.IsSuccessStatusCode)
                        {
                            response = JsonConvert.DeserializeObject<Response>(responseMsg.Content.ReadAsStringAsync().Result);
                            response.ResponseID = SessionID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log.Error("PackUpController.GetSource Completed", ex);
                return _CheckUpService.GetErrorResponse(ex.Message);
            }
            _logger.Log.Info($"Post Complete with Response: {response.ErrorCode}: {response.ErrorDesc}");
            return response;
        }

        [HttpGet]
        public async Task<Response> GetIsMemberOrg(string username, string source)
        {
            username = cryptor.DecryptDES(username);
            source = cryptor.DecryptDES(source);
            Response response = new Response();
            try
            {
                HttpClient httpClient = new HttpClient();
                var parameters = $"?userName={username}";
                HttpResponseMessage responseMsg = httpClient.GetAsync($"{_configuration.CheckUpBackEndURL}/CheckUp/RestrictPermissions{parameters}").Result;
                if (responseMsg.IsSuccessStatusCode)
                {
                    response = JsonConvert.DeserializeObject<Response>(responseMsg.Content.ReadAsStringAsync().Result);
                    if (response.ErrorCode == 200)
                    {
                        string SessionID = response.ResponseID;
                        //1. Get Source
                        httpClient = new HttpClient();
                        parameters = $"?userName={username}&source={source}";
                        responseMsg = httpClient.GetAsync($"{_configuration.CheckUpBackEndURL}/CheckUp/GetIsMemberOrg{parameters}").Result;
                        if (responseMsg.IsSuccessStatusCode)
                        {
                            response = JsonConvert.DeserializeObject<Response>(responseMsg.Content.ReadAsStringAsync().Result);
                            response.ResponseID = SessionID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Log.Error("PackUpController.IsMemberOrg Completed", ex);
                return _CheckUpService.GetErrorResponse(ex.Message);
            }
            _logger.Log.Info($"Post Complete with Response: {response.ErrorCode}: {response.ErrorDesc}");
            return response;
        }
        public static string RemoveWhitespace( string input)
        {
            input = input.Replace(" ", "");
            return input;
        }
    }
}