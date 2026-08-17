using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AcraValidatorWebService.Controllers
{
    public class ValidatorServiceController : Controller
    {
        private readonly AcraIDServices.AcraIdentityValidatorService _acraIdentityValidatorService;
        private readonly AcraUtils.Logger _logger;
        private readonly ILogger<ValidatorServiceController> _msLogger;

        public ValidatorServiceController(
            AcraIDServices.AcraIdentityValidatorService acraIdentityValidatorService,
            AcraUtils.Logger logger,
            ILogger<ValidatorServiceController> msLogger)
        {
            _acraIdentityValidatorService = acraIdentityValidatorService;
            _logger = logger;
            _msLogger = msLogger;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Start()
        {
            try
            {
                _acraIdentityValidatorService.Start();
                _msLogger.LogInformation("AcraIdentityValidatorService started");
            }
            catch (Exception ex)
            {
                _logger.Log.Error($"AcraIdentityValidatorService.Start failed: {ex.Message}");
                _msLogger.LogError(ex, "AcraIdentityValidatorService.Start failed");
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Stop()
        {
            try
            {
                _acraIdentityValidatorService.Stop();
                _msLogger.LogInformation("AcraIdentityValidatorService stopped");
            }
            catch (Exception ex)
            {
                _logger.Log.Error($"AcraIdentityValidatorService.Stop failed: {ex.Message}");
                _msLogger.LogError(ex, "AcraIdentityValidatorService.Stop failed");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
