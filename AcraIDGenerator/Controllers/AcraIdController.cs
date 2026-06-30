using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcraData.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcraIDGenerator.Controllers
{
    public class AcraIdController : Controller
    {
        AcraIDGeneratorService _acraIdGeneratorService;
        Task task;

        public AcraIdController(AcraIDGenerator.AcraIDGeneratorService acraIDGeneratorService)
        {
            _acraIdGeneratorService = acraIDGeneratorService;
        }
        public ActionResult Index()
        
        {
            return View();
        }
        public ActionResult Started()
        {
            return View("Started");
        }
        public IActionResult Start()
        {
            _acraIdGeneratorService.start();
            //_acraIdGeneratorService.AcraIdGenerate();
            return RedirectToAction(nameof(Started));
        }
        public ActionResult OneTimeStarted()
        {
            
            return View("OneTimeStarted");
        }
        public IActionResult OneTimeStart()
        {
            task = new Task(() => _acraIdGeneratorService.OneTimeStart());
            task.Start();
            
            //_acraIdGeneratorService.OneTimeStart();      
            return RedirectToAction(nameof(OneTimeStarted)); 
        }
        public ActionResult Status()
        {
            
            if (task == null)
                View("CheckStatus").ViewData["Status"] = "STATUS: Not Started";
            else
                View("CheckStatus").ViewData["Status"] = task.Status;
            return View("CheckStatus");
        }
    }
}