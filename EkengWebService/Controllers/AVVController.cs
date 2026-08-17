using System;
using AcraIDServices;
using AcraIDServices.Models.AVV;
using Microsoft.AspNetCore.Mvc;
using AcraUtils;
using static AcraIDServices.AVVClient;
using Microsoft.Extensions.Logging;

namespace EkengWebService.Controllers
{
    public class AVVController : Controller
    {
        private readonly AVVClient _AVVClient;
        private readonly Logger _logger;
        private readonly ILogger<AVVController> _msLogger;

        public AVVController(AVVClient avvClient, Logger logger, ILogger<AVVController> msLogger)
        {
            _AVVClient = avvClient;
            _logger = logger;
            _msLogger = msLogger;
        }

        public IActionResult Index()
        {
            return View();
        }

        private JsonResult OkOrError(object data, string operation, string requestKey = null)
        {
            if (data != null)
            {
                _msLogger.LogInformation("{Operation} succeeded. Key={Key}", operation, requestKey);
                return Json(data);
            }

            _msLogger.LogWarning("{Operation} returned empty/null data. Key={Key}", operation, requestKey);
            return Json(new
            {
                status = "ERROR",
                errorCode = "NO_DATA",
                message = "External service returned no data or non-success status"
            });
        }

        private JsonResult ErrorResult(Exception ex, string operation, string requestKey = null)
        {
            _logger.Log.Error($"{operation} failed. Key={requestKey}. Error: {ex.Message}");
            _msLogger.LogError(ex, "{Operation} failed. Key={Key}", operation, requestKey);

            return Json(new
            {
                status = "ERROR",
                errorCode = "EXCEPTION",
                message = ex.Message
            });
        }

        [HttpPost]
        public JsonResult GetPersonInfoBySSN([FromBody] BySSN personData)
        {
            if (personData == null || string.IsNullOrWhiteSpace(personData.psn))
            {
                _msLogger.LogWarning("AVV GetPersonInfoBySSN called with null/empty psn");
                return Json(new { status = "ERROR", errorCode = "INVALID_INPUT", message = "PSN is required" });
            }

            try
            {
                _AVVClient.GetPersonData(RequestType.SSN, personData);

                if (_AVVClient.Response != null && _AVVClient.Response.IsSuccessStatusCode)
                    return OkOrError(_AVVClient.Data, "AVV.GetPersonInfoBySSN", personData.psn);

                var status = _AVVClient.Response?.StatusCode.ToString() ?? "null";
                _msLogger.LogWarning("AVV GetPersonInfoBySSN non-success HTTP status={Status}. PSN={Psn}", status, personData.psn);
                return Json(new
                {
                    status = "ERROR",
                    errorCode = "HTTP_" + status,
                    message = "AVV service returned non-success status"
                });
            }
            catch (Exception ex)
            {
                return ErrorResult(ex, "AVV.GetPersonInfoBySSN", personData.psn);
            }
        }

        [HttpPost]
        public JsonResult GetPersonInfoByDocument([FromBody] ByDocument personData)
        {
            if (personData == null || string.IsNullOrWhiteSpace(personData.docnum))
            {
                _msLogger.LogWarning("AVV GetPersonInfoByDocument called with null/empty docnum");
                return Json(new { status = "ERROR", errorCode = "INVALID_INPUT", message = "Document number is required" });
            }

            try
            {
                _AVVClient.GetPersonData(RequestType.Document, personData);

                if (_AVVClient.Response != null && _AVVClient.Response.IsSuccessStatusCode)
                    return OkOrError(_AVVClient.Data, "AVV.GetPersonInfoByDocument", personData.docnum);

                var status = _AVVClient.Response?.StatusCode.ToString() ?? "null";
                _msLogger.LogWarning("AVV GetPersonInfoByDocument non-success HTTP status={Status}. Doc={Doc}", status, personData.docnum);
                return Json(new
                {
                    status = "ERROR",
                    errorCode = "HTTP_" + status,
                    message = "AVV service returned non-success status"
                });
            }
            catch (Exception ex)
            {
                return ErrorResult(ex, "AVV.GetPersonInfoByDocument", personData.docnum);
            }
        }

        [HttpPost]
        public JsonResult GetPersonInfoByNames([FromBody] ByName personData)
        {
            if (personData == null || string.IsNullOrWhiteSpace(personData.first_name) || string.IsNullOrWhiteSpace(personData.last_name))
            {
                _msLogger.LogWarning("AVV GetPersonInfoByNames called with incomplete name data");
                return Json(new { status = "ERROR", errorCode = "INVALID_INPUT", message = "first_name and last_name are required" });
            }

            try
            {
                // Fixed: previously this method never called GetPersonData
                _AVVClient.GetPersonData(RequestType.Name, personData);

                if (_AVVClient.Response != null && _AVVClient.Response.IsSuccessStatusCode)
                    return OkOrError(_AVVClient.Data, "AVV.GetPersonInfoByNames", $"{personData.last_name} {personData.first_name}");

                var status = _AVVClient.Response?.StatusCode.ToString() ?? "null";
                _msLogger.LogWarning("AVV GetPersonInfoByNames non-success HTTP status={Status}", status);
                return Json(new
                {
                    status = "ERROR",
                    errorCode = "HTTP_" + status,
                    message = "AVV service returned non-success status"
                });
            }
            catch (Exception ex)
            {
                return ErrorResult(ex, "AVV.GetPersonInfoByNames", $"{personData?.last_name} {personData?.first_name}");
            }
        }
    }
}
