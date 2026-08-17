using AcraIDServices;
using AcraIDServices.Models;
using AcraUtils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AcraValidatorWebService.Controllers
{
    public class ValidatorController : Controller
    {
        private readonly AcraIdentityValidatorEkengModel _acraIdentityValidatorService;
        private readonly ConverterModel _converterModel;
        private readonly Logger _logger;
        private readonly ILogger<ValidatorController> _msLogger;

        public ValidatorController(
            AcraIdentityValidatorEkengModel acraIdentityValidatorService,
            ConverterModel converterModel,
            Logger logger,
            ILogger<ValidatorController> msLogger)
        {
            _acraIdentityValidatorService = acraIdentityValidatorService;
            _converterModel = converterModel;
            _logger = logger;
            _msLogger = msLogger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ValidationBySSN(string SSN, int PersonID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SSN))
                {
                    _msLogger.LogWarning("ValidationBySSN called with empty SSN. PersonID={PersonID}", PersonID);
                    return View();
                }

                _acraIdentityValidatorService.AcraIdentityValidator(SSN, PersonID);
                _msLogger.LogInformation("ValidationBySSN completed. SSN={Ssn}, PersonID={PersonID}", SSN, PersonID);
            }
            catch (Exception ex)
            {
                _logger.Log.Error($"ValidationBySSN failed. SSN={SSN}, PersonID={PersonID}. Error: {ex.Message}");
                _msLogger.LogError(ex, "ValidationBySSN failed. SSN={Ssn}, PersonID={PersonID}", SSN, PersonID);
            }

            return View();
        }

        [HttpPost]
        public JsonResult ConverterValidation([FromBody] ConverterPersonInfo PersonInfo)
        {
            try
            {
                if (PersonInfo == null)
                {
                    _msLogger.LogWarning("ConverterValidation called with null body");
                    return Json(new ConverterRespModel { Status = 1, ErrorTypes = new System.Collections.Generic.List<ErrorType> { ErrorType.Ekeng } });
                }

                var result = _converterModel.VerifyInfo(PersonInfo);
                _msLogger.LogInformation("ConverterValidation completed. Status={Status}", result?.Status);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.Log.Error($"ConverterValidation failed: {ex.Message}");
                _msLogger.LogError(ex, "ConverterValidation failed");
                return Json(new ConverterRespModel { Status = 1, ErrorTypes = new System.Collections.Generic.List<ErrorType> { ErrorType.Ekeng } });
            }
        }

        public JsonResult ValidateWithoutResidency(string passport, string firstName, string lastName, string birthDate, string ssn, string idCard)
        {
            try
            {
                var personWR = new PersonWithoutResidency
                {
                    firstName = firstName,
                    lastName = lastName,
                    passport = passport,
                    ssn = ssn,
                    birthDate = birthDate,
                    idCard = idCard
                };

                var result = _converterModel.ValidateWithoutResidency(personWR);
                _msLogger.LogInformation("ValidateWithoutResidency completed. Passport={Passport}", passport);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.Log.Error($"ValidateWithoutResidency failed. Passport={passport}. Error: {ex.Message}");
                _msLogger.LogError(ex, "ValidateWithoutResidency failed. Passport={Passport}", passport);
                return Json(new NonResidentRespModel { EkengStatus = false, IsValid = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Validate([FromBody] RequestModel model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.passport))
                {
                    _msLogger.LogWarning("Validate called with null/empty passport");
                    return Json(new NonResidentRespModel { EkengStatus = false, IsValid = false });
                }

                var result = await _converterModel.Validate(model.passport);
                _msLogger.LogInformation("Validate completed. Passport={Passport}", model.passport);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.Log.Error($"Validate failed. Passport={model?.passport}. Error: {ex.Message}");
                _msLogger.LogError(ex, "Validate failed. Passport={Passport}", model?.passport);
                return Json(new NonResidentRespModel { EkengStatus = false, IsValid = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetInfoBySSN([FromBody] RequestModelBySSN model)
        {
            try
            {
                if (model == null || string.IsNullOrWhiteSpace(model.SSN))
                {
                    _msLogger.LogWarning("GetInfoBySSN called with null/empty SSN");
                    return Json(new NonResidentRespModel { EkengStatus = false, IsValid = false });
                }

                var result = await _converterModel.GetPersonInfoBySSN(model.SSN);
                _msLogger.LogInformation("GetInfoBySSN completed. SSN={Ssn}, EkengStatus={Status}", model.SSN, result?.EkengStatus);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.Log.Error($"GetInfoBySSN failed. SSN={model?.SSN}. Error: {ex.Message}");
                _msLogger.LogError(ex, "GetInfoBySSN failed. SSN={Ssn}", model?.SSN);
                return Json(new NonResidentRespModel { EkengStatus = false, IsValid = false });
            }
        }
    }
}
