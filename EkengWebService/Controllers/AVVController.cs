using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcraIDServices;
using AcraIDServices.Models.AVV;
using Microsoft.AspNetCore.Mvc;
using AcraUtils;
using static AcraIDServices.AVVClient;
using Newtonsoft.Json;

namespace EkengWebService.Controllers
{
    public class AVVController : Controller
    {
        AVVClient _AVVClient;
        Logger _logger;
        public AVVController(AVVClient avvClient, Logger logger)
        {
            _AVVClient = avvClient;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetPersonInfoBySSN([FromBody] BySSN personData)
        {
            EkengResponse aVVResponse = new EkengResponse();
            try
            {
                _AVVClient.GetPersonData(RequestType.SSN, personData);
                if (_AVVClient.Response.IsSuccessStatusCode)
                {
                    return Json(_AVVClient.Data);
                    //aVVResponse = JsonConvert.DeserializeObject<EkengResponse>(_EkengClient.Response.Content.ReadAsStringAsync().Result);

                    //if (aVVResponse.status == "OK")
                    //{
                    //    AcraUtils.OpenSSLAES256Cryptor cryptor = new OpenSSLAES256Cryptor();
                    //    // Decode Response
                    //}
                    //else
                    //{
                    //    //Error
                    //}
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                // return Json(_VxClient.Response.Content.ReadAsStringAsync().Result);
            }

            return Json(aVVResponse);
        }

        public JsonResult GetPersonInfoByDocument([FromBody] ByDocument personData)
        {
            EkengResponse aVVResponse = new EkengResponse();
            try
            {
                _AVVClient.GetPersonData(RequestType.Document, personData);
                if (_AVVClient.Response.IsSuccessStatusCode)
                {
                    return Json(_AVVClient.Data);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                // return Json(_VxClient.Response.Content.ReadAsStringAsync().Result);
            }

            return Json(aVVResponse);
        }

        public IActionResult GetPersonInfoByNames([FromBody] ByName personData)
        {
            EkengResponse aVVResponse = new EkengResponse();
            try
            {
                if (_AVVClient.Response.IsSuccessStatusCode)
                {
                    return Json(_AVVClient.Data);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                // return Json(_VxClient.Response.Content.ReadAsStringAsync().Result);
            }

            return Json(aVVResponse);
        }
    }
}
