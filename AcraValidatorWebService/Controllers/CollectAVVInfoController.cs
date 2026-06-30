using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AcraValidatorWebService.Controllers
{    
    public class CollectAVVInfoController : Controller
    {
        AcraIDServices.CollectAVVInfoService _collectAVVInfoService;
        AcraUtils.Logger _logger;
        public CollectAVVInfoController(AcraIDServices.CollectAVVInfoService collectAVVInfoService, AcraUtils.Logger logger)
        {
            _collectAVVInfoService = collectAVVInfoService;
            _logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Start()
        {
            _collectAVVInfoService.Start();
            return RedirectToAction(nameof(Index));
        }


        public ActionResult Stop()
        {
            _collectAVVInfoService.Stop();
            return RedirectToAction(nameof(Index));
        }
    }
}