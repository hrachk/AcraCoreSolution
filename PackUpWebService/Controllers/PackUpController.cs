using System;
using System.Net.Http;
using System.Threading.Tasks;
using CheckUpService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AcraUtils.Configuration;
using Newtonsoft.Json;

namespace CheckUpWebService.Controllers
{
    public class PackUpController : Controller
    {
        private readonly AcraUtils.Logger _logger;
        private readonly ILogger<PackUpController> _msLogger;
        private readonly CheckUpService.CheckUpService _CheckUpService;
        private readonly PackUpConfig _configuration;
        private readonly AcraUtils.Cryptor _cryptor = new AcraUtils.Cryptor();

        public PackUpController(
            CheckUpService.CheckUpService CheckUpService,
            AcraUtils.Logger logger,
            ILogger<PackUpController> msLogger,
            IOptions<PackUpConfig> configuration)
        {
            _CheckUpService = CheckUpService;
            _configuration = configuration.Value;
            _logger = logger;
            _msLogger = msLogger;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>Safe DES decrypt – returns null and logs on failure instead of throwing.</summary>
        private string SafeDecrypt(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            try
            {
                value = value.Replace(" ", "+");
                return _cryptor.DecryptDES(value);
            }
            catch (Exception ex)
            {
                _msLogger.LogError(ex, "Failed to decrypt field {FieldName}", fieldName);
                _logger.Log.Error($"Decrypt failed for {fieldName}: {ex.Message}");
                return null;
            }
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<Response> Post(string sourceName, string username, string sessionID, string thumbprint, string version)
        {
            Response response = new Response();

            try
            {
                sourceName = SafeDecrypt(sourceName, nameof(sourceName));
                username = SafeDecrypt(username, nameof(username));
                sessionID = SafeDecrypt(sessionID, nameof(sessionID));
                thumbprint = SafeDecrypt(thumbprint, nameof(thumbprint));
                version = SafeDecrypt(version, nameof(version));

                if (sourceName == null || username == null || sessionID == null || thumbprint == null || version == null)
                {
                    response.ErrorCode = 101;
                    response.ErrorDesc = "Invalid encrypted parameters";
                    response.ResponseTime = DateTime.Now.Ticks;
                    _msLogger.LogWarning("Post rejected: decrypt failed for one or more parameters");
                    return response;
                }

                sessionID = RemoveWhitespace(sessionID);

                if (version != _configuration.VersionControl)
                {
                    response.ResponseMessage = "Առկա է տվյալ ծրագրի նոր տարբերակ, անհրաժեշտ է թարմացնել";
                    response.ErrorCode = 1025;
                    response.ResponseTime = DateTime.Now.Ticks;
                    _msLogger.LogWarning("Post rejected: version mismatch. Client={ClientVersion}, Expected={Expected}", version, _configuration.VersionControl);
                    return response;
                }

                if (_configuration.Switch == false)
                {
                    response.ResponseMessage = $"Տվյալ պահին վերբեռնումը դադարեցված է, խնդրում եմ կրկին փորձել {_configuration.TimeOfReopening}-ից հետո";
                    response.ErrorCode = 1024;
                    response.ResponseTime = DateTime.Now.Ticks;
                    _msLogger.LogWarning("Post rejected: upload switch is OFF");
                    return response;
                }

                if (Request.Form?.Files == null || Request.Form.Files.Count == 0)
                {
                    response.ErrorCode = 206;
                    response.ErrorDesc = "No file uploaded";
                    response.ResponseTime = DateTime.Now.Ticks;
                    _msLogger.LogWarning("Post rejected: no file in request. User={User}", username);
                    return response;
                }

                var file = Request.Form.Files[0];
                _msLogger.LogInformation("Post started. User={User}, Source={Source}, File={File}, Session={Session}",
                    username, sourceName, file.FileName, sessionID);

                // 1. Upload File
                response = await _CheckUpService.Upload(file);

                if (response.ErrorCode == 200)
                {
                    // 2. Register Received File via CheckUpBackEnd
                    using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
                    var parameters =
                        $"?sessionID={Uri.EscapeDataString(sessionID)}" +
                        $"&userName={Uri.EscapeDataString(username)}" +
                        $"&sourceName={Uri.EscapeDataString(sourceName)}" +
                        $"&path={Uri.EscapeDataString(_configuration.Destination[0])}" +
                        $"&fileName={Uri.EscapeDataString(file.FileName)}" +
                        $"&thumbprint={Uri.EscapeDataString(thumbprint)}";

                    var responseMsg = await httpClient.GetAsync(
                        $"{_configuration.CheckUpBackEndURL}/CheckUp/RegisterReceivedPackage{parameters}");

                    if (responseMsg.IsSuccessStatusCode)
                    {
                        var body = await responseMsg.Content.ReadAsStringAsync();
                        response = JsonConvert.DeserializeObject<Response>(body) ?? response;
                    }
                    else
                    {
                        _msLogger.LogError("RegisterReceivedPackage failed. HTTP={Status}, User={User}, File={File}",
                            responseMsg.StatusCode, username, file.FileName);
                        response.ErrorCode = 206;
                        response.ErrorDesc = $"RegisterReceivedPackage HTTP {(int)responseMsg.StatusCode}";
                    }
                }

                _msLogger.LogInformation("Post completed. User={User}, File={File}, ErrorCode={Code}, ErrorDesc={Desc}",
                    username, file.FileName, response.ErrorCode, response.ErrorDesc);
                _logger.Log.Info($"Post Complete with Response: {response.ErrorCode}: {response.ErrorDesc}");
            }
            catch (Exception ex)
            {
                _logger.Log.Error("PackUpController.Post failed", ex);
                _msLogger.LogError(ex, "PackUpController.Post failed");
                return _CheckUpService.GetErrorResponse(ex.Message);
            }

            response.ResponseTime = DateTime.Now.Ticks;
            return response;
        }

        [HttpGet]
        public async Task<Response> GetSource(string username)
        {
            Response response = new Response();
            try
            {
                username = SafeDecrypt(username, nameof(username));
                if (string.IsNullOrWhiteSpace(username))
                {
                    response.ErrorCode = 101;
                    response.ErrorDesc = "Invalid username";
                    response.ResponseTime = DateTime.Now.Ticks;
                    return response;
                }

                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
                var parameters = $"?userName={Uri.EscapeDataString(username)}";
                var responseMsg = await httpClient.GetAsync(
                    $"{_configuration.CheckUpBackEndURL}/CheckUp/RestrictPermissions{parameters}");

                if (!responseMsg.IsSuccessStatusCode)
                {
                    _msLogger.LogError("GetSource RestrictPermissions HTTP={Status}, User={User}", responseMsg.StatusCode, username);
                    return _CheckUpService.GetErrorResponse($"RestrictPermissions HTTP {(int)responseMsg.StatusCode}");
                }

                var body = await responseMsg.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<Response>(body) ?? new Response();

                if (response.ErrorCode == 200)
                {
                    string sessionId = response.ResponseID;
                    responseMsg = await httpClient.GetAsync(
                        $"{_configuration.CheckUpBackEndURL}/CheckUp/GetSource{parameters}");

                    if (responseMsg.IsSuccessStatusCode)
                    {
                        body = await responseMsg.Content.ReadAsStringAsync();
                        response = JsonConvert.DeserializeObject<Response>(body) ?? response;
                        response.ResponseID = sessionId;
                    }
                    else
                    {
                        _msLogger.LogError("GetSource HTTP={Status}, User={User}", responseMsg.StatusCode, username);
                        response = _CheckUpService.GetErrorResponse($"GetSource HTTP {(int)responseMsg.StatusCode}");
                        response.ResponseID = sessionId;
                    }
                }

                _msLogger.LogInformation("GetSource completed. User={User}, ErrorCode={Code}", username, response.ErrorCode);
                _logger.Log.Info($"GetSource Complete with Response: {response.ErrorCode}: {response.ErrorDesc}");
            }
            catch (Exception ex)
            {
                _logger.Log.Error("PackUpController.GetSource failed", ex);
                _msLogger.LogError(ex, "PackUpController.GetSource failed");
                return _CheckUpService.GetErrorResponse(ex.Message);
            }

            response.ResponseTime = DateTime.Now.Ticks;
            return response;
        }

        [HttpGet]
        public async Task<Response> GetIsMemberOrg(string username, string source)
        {
            Response response = new Response();
            try
            {
                username = SafeDecrypt(username, nameof(username));
                source = SafeDecrypt(source, nameof(source));

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(source))
                {
                    response.ErrorCode = 101;
                    response.ErrorDesc = "Invalid username or source";
                    response.ResponseTime = DateTime.Now.Ticks;
                    return response;
                }

                using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
                var parameters = $"?userName={Uri.EscapeDataString(username)}";
                var responseMsg = await httpClient.GetAsync(
                    $"{_configuration.CheckUpBackEndURL}/CheckUp/RestrictPermissions{parameters}");

                if (!responseMsg.IsSuccessStatusCode)
                {
                    _msLogger.LogError("GetIsMemberOrg RestrictPermissions HTTP={Status}, User={User}", responseMsg.StatusCode, username);
                    return _CheckUpService.GetErrorResponse($"RestrictPermissions HTTP {(int)responseMsg.StatusCode}");
                }

                var body = await responseMsg.Content.ReadAsStringAsync();
                response = JsonConvert.DeserializeObject<Response>(body) ?? new Response();

                if (response.ErrorCode == 200)
                {
                    string sessionId = response.ResponseID;
                    parameters = $"?userName={Uri.EscapeDataString(username)}&source={Uri.EscapeDataString(source)}";
                    responseMsg = await httpClient.GetAsync(
                        $"{_configuration.CheckUpBackEndURL}/CheckUp/GetIsMemberOrg{parameters}");

                    if (responseMsg.IsSuccessStatusCode)
                    {
                        body = await responseMsg.Content.ReadAsStringAsync();
                        response = JsonConvert.DeserializeObject<Response>(body) ?? response;
                        response.ResponseID = sessionId;
                    }
                    else
                    {
                        _msLogger.LogError("GetIsMemberOrg HTTP={Status}, User={User}, Source={Source}",
                            responseMsg.StatusCode, username, source);
                        response = _CheckUpService.GetErrorResponse($"GetIsMemberOrg HTTP {(int)responseMsg.StatusCode}");
                        response.ResponseID = sessionId;
                    }
                }

                _msLogger.LogInformation("GetIsMemberOrg completed. User={User}, Source={Source}, ErrorCode={Code}",
                    username, source, response.ErrorCode);
                _logger.Log.Info($"GetIsMemberOrg Complete with Response: {response.ErrorCode}: {response.ErrorDesc}");
            }
            catch (Exception ex)
            {
                _logger.Log.Error("PackUpController.GetIsMemberOrg failed", ex);
                _msLogger.LogError(ex, "PackUpController.GetIsMemberOrg failed");
                return _CheckUpService.GetErrorResponse(ex.Message);
            }

            response.ResponseTime = DateTime.Now.Ticks;
            return response;
        }

        public static string RemoveWhitespace(string input)
        {
            return string.IsNullOrEmpty(input) ? input : input.Replace(" ", "");
        }
    }
}
