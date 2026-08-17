using System;
using AcraIDServices;
using AcraIDServices.Models;
using Microsoft.AspNetCore.Mvc;
using AcraUtils;
using static AcraIDServices.EkengClient;
using Microsoft.Extensions.Logging;

namespace EkengWebService.Controllers
{
    public class EkengController : Controller
    {
        private readonly EkengClient _EkengClient;
        private readonly Logger _logger;
        private readonly ILogger<EkengController> _msLogger;

        public EkengController(EkengClient ekengClient, Logger logger, ILogger<EkengController> msLogger)
        {
            _EkengClient = ekengClient;
            _logger = logger;
            _msLogger = msLogger;
        }

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Helper: never return null to client. Always log outcome.
        /// </summary>
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
            if (personData == null || string.IsNullOrWhiteSpace(personData.ssn))
            {
                _msLogger.LogWarning("GetPersonInfoBySSN called with null/empty SSN");
                return Json(new { status = "ERROR", errorCode = "INVALID_INPUT", message = "SSN is required" });
            }

            try
            {
                _EkengClient.GetPersonData(RequestType.SSN, personData);

                if (_EkengClient.Response != null && _EkengClient.Response.IsSuccessStatusCode)
                    return OkOrError(_EkengClient.Data, "GetPersonInfoBySSN", personData.ssn);

                var status = _EkengClient.Response?.StatusCode.ToString() ?? "null";
                _msLogger.LogWarning("GetPersonInfoBySSN non-success HTTP status={Status}. SSN={Ssn}", status, personData.ssn);
                return Json(new
                {
                    status = "ERROR",
                    errorCode = "HTTP_" + status,
                    message = "Ekeng service returned non-success status"
                });
            }
            catch (Exception ex)
            {
                return ErrorResult(ex, "GetPersonInfoBySSN", personData.ssn);
            }
        }

        [HttpPost]
        public JsonResult GetPersonInfoByDocument([FromBody] ByDocument personData)
        {
            if (personData == null || string.IsNullOrWhiteSpace(personData.documentNumber))
            {
                _msLogger.LogWarning("GetPersonInfoByDocument called with null/empty documentNumber");
                return Json(new { status = "ERROR", errorCode = "INVALID_INPUT", message = "Document number is required" });
            }

            try
            {
                _EkengClient.GetPersonData(RequestType.Document, personData);

                if (_EkengClient.Response != null && _EkengClient.Response.IsSuccessStatusCode)
                    return OkOrError(_EkengClient.Data, "GetPersonInfoByDocument", personData.documentNumber);

                var status = _EkengClient.Response?.StatusCode.ToString() ?? "null";
                _msLogger.LogWarning("GetPersonInfoByDocument non-success HTTP status={Status}. Doc={Doc}", status, personData.documentNumber);
                return Json(new
                {
                    status = "ERROR",
                    errorCode = "HTTP_" + status,
                    message = "Ekeng service returned non-success status"
                });
            }
            catch (Exception ex)
            {
                return ErrorResult(ex, "GetPersonInfoByDocument", personData.documentNumber);
            }
        }

        [HttpPost]
        public JsonResult GetPersonInfoByNames([FromBody] ByName personData)
        {
            if (personData == null || string.IsNullOrWhiteSpace(personData.name) || string.IsNullOrWhiteSpace(personData.surname))
            {
                _msLogger.LogWarning("GetPersonInfoByNames called with incomplete name data");
                return Json(new { status = "ERROR", errorCode = "INVALID_INPUT", message = "Name and surname are required" });
            }

            try
            {
                // Fixed: previously this method never called GetPersonData
                _EkengClient.GetPersonData(RequestType.Name, personData);

                if (_EkengClient.Response != null && _EkengClient.Response.IsSuccessStatusCode)
                    return OkOrError(_EkengClient.Data, "GetPersonInfoByNames", $"{personData.surname} {personData.name}");

                var status = _EkengClient.Response?.StatusCode.ToString() ?? "null";
                _msLogger.LogWarning("GetPersonInfoByNames non-success HTTP status={Status}", status);
                return Json(new
                {
                    status = "ERROR",
                    errorCode = "HTTP_" + status,
                    message = "Ekeng service returned non-success status"
                });
            }
            catch (Exception ex)
            {
                return ErrorResult(ex, "GetPersonInfoByNames", $"{personData?.surname} {personData?.name}");
            }
        }
    }
}
