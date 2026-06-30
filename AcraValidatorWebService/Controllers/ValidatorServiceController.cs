using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AcraValidatorWebService.Controllers
{    
    public class ValidatorServiceController : Controller
    {
        AcraIDServices.AcraIdentityValidatorService _acraIdentityValidatorService;
        AcraUtils.Logger _logger;
        public ValidatorServiceController(AcraIDServices.AcraIdentityValidatorService acraIdentityValidatorService, AcraUtils.Logger logger)
        {
            _acraIdentityValidatorService = acraIdentityValidatorService;
            _logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Start()
        {
            _acraIdentityValidatorService.Start();
            return RedirectToAction(nameof(Index));
        }


        public ActionResult Stop()
        {
            _acraIdentityValidatorService.Stop();
            return RedirectToAction(nameof(Index));
        }
    }
}