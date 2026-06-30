using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AcraIDServices;
using AcraIDServices.Models;
using Microsoft.AspNetCore.Mvc;
using AcraUtils;
using static AcraIDServices.EkengClient;
using Newtonsoft.Json;

namespace EkengWebService.Controllers
{
    public class EkengController : Controller
    {
        EkengClient _EkengClient;
        Logger _logger;
        public EkengController(EkengClient ekengClient, Logger logger)
        {
            _EkengClient = ekengClient;
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

                _EkengClient.GetPersonData(RequestType.SSN, personData);
                if (_EkengClient.Response.IsSuccessStatusCode)
                {
                    return Json(_EkengClient.Data);
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
                _EkengClient.GetPersonData(RequestType.Document, personData);
                if (_EkengClient.Response.IsSuccessStatusCode)
                {
                    return Json(_EkengClient.Data);                   
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
                if (_EkengClient.Response.IsSuccessStatusCode)
                {
                    return Json(_EkengClient.Data);
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
