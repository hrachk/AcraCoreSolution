using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AcraIdentityFE.Models;
using System.Net.Http;
using IdentityModel.Client;
using Microsoft.Extensions.Options;

namespace AcraIdentityFE.Controllers
{
    public class HomeController : Controller
    {        

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public async Task<string> LiveCheck()
        {
            return "Yes";
        }

        
    }
}
