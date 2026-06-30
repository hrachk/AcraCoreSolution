using AcraIDServices;
using AcraIDServices.Models;
using AcraUtils;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AcraValidatorWebService.Controllers
{
    public class ValidatorController : Controller
    {
        AcraIdentityValidatorEkengModel _acraIdentityValidatorService;
        AcraIDServices.ConverterModel _converterModel;
        Logger _logger;

        public ValidatorController(AcraIdentityValidatorEkengModel acraIdentityValidatorService, AcraIDServices.ConverterModel converterModel, Logger logger)
        {
            _acraIdentityValidatorService = acraIdentityValidatorService;
            _converterModel = converterModel;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ValidationBySSN(string SSN, int PersonID)
        {
            _acraIdentityValidatorService.AcraIdentityValidator(SSN, PersonID);
            return View();
        }

        [HttpPost]
        public JsonResult ConverterValidation([FromBody] ConverterPersonInfo PersonInfo)
        {
            return Json(_converterModel.VerifyInfo(PersonInfo));
        }
        public JsonResult ValidateWithoutResidency(string passport, string firstName, string lastName, string birthDate, string ssn, string idCard)
        {
            PersonWithoutResidency personWR = new PersonWithoutResidency() { firstName = firstName, lastName = lastName, passport = passport, ssn = ssn, birthDate = birthDate, idCard = idCard };
            return Json(_converterModel.ValidateWithoutResidency(personWR));

        }

        [HttpPost]
        public async Task<IActionResult> Validate([FromBody] RequestModel model)
        {
             return Json(await _converterModel.Validate(model.passport));

        }

        [HttpPost]
        public async Task<IActionResult> GetInfoBySSN([FromBody] RequestModelBySSN model)
        {
            return Json(await _converterModel.GetPersonInfoBySSN(model.SSN));

        }


    }
}