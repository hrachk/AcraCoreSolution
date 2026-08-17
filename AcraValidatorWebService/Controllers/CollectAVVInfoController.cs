using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AcraValidatorWebService.Controllers
{
    public class CollectAVVInfoController : Controller
    {
        private readonly AcraIDServices.CollectAVVInfoService _collectAVVInfoService;
        private readonly AcraUtils.Logger _logger;
        private readonly ILogger<CollectAVVInfoController> _msLogger;

        public CollectAVVInfoController(
            AcraIDServices.CollectAVVInfoService collectAVVInfoService,
            AcraUtils.Logger logger,
            ILogger<CollectAVVInfoController> msLogger)
        {
            _collectAVVInfoService = collectAVVInfoService;
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
                _collectAVVInfoService.Start();
                _msLogger.LogInformation("CollectAVVInfoService started");
            }
            catch (Exception ex)
            {
                _logger.Log.Error($"CollectAVVInfoService.Start failed: {ex.Message}");
                _msLogger.LogError(ex, "CollectAVVInfoService.Start failed");
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Stop()
        {
            try
            {
                _collectAVVInfoService.Stop();
                _msLogger.LogInformation("CollectAVVInfoService stopped");
            }
            catch (Exception ex)
            {
                _logger.Log.Error($"CollectAVVInfoService.Stop failed: {ex.Message}");
                _msLogger.LogError(ex, "CollectAVVInfoService.Stop failed");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
