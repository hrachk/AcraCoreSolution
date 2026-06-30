using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AcraIDGenerator.Models;
using System.Net.Http;
using AcraUtils.Configuration;

namespace AcraIDGenerator.Controllers
{
    public class HomeController : Controller
    {
        AcraIDConfig _acraIDConfig;
        public HomeController(AcraIDConfig acraIDConfig)
        {
            _acraIDConfig = acraIDConfig;
        }
        public IActionResult Index()
        {
            /*var url = $"http://{_acraIDConfig.IPv4}:{_acraIDConfig.Port}/Generator/Generate";
            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 1, 0);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url) { };*/
            //HttpResponseMessage response = client.SendAsync(request).Result;
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
