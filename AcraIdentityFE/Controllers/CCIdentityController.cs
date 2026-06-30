using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AcraUtils;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;


namespace AcraIdentityFE.Controllers
{
    public class CCIdentityController : Controller
    {
        private Logger _logger;
        private AcraUtils.Configuration.AcraIdentityConfig _configuration;

        public CCIdentityController(Logger logger, IOptions<AcraUtils.Configuration.AcraIdentityConfig> configuration)
        {
            _configuration = configuration.Value;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public string GetAccessToken(string ClientID, string ClientSecret, string Scope)
        {
            // It's Work
            //var client = new RestClient($"{_configuration.IdentityServerUrl}");

            //var request = new RestRequest(Method.POST);

            //request.AddHeader("cache-control", "no-cache");

            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            //request.AddParameter("undefined", "grant_type=password&client_id=CheckUpClient&password=123456&client_secret=c7b4e734-2788-45be-94f5-905711f8d243&username=a&undefined=", ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);

            //return response.Content;

            HttpClient client = new HttpClient();
            var tokenResponse = client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = $"{_configuration.IdentityServerUrl}",
                ClientId = ClientID,
                ClientSecret = ClientSecret,                
                Scope = Scope,
                GrantType = "client_credentials"
            }).Result;

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                _logger.Log.Error($"Client Credentials GetAccessToken Failed Error: {tokenResponse.Error}");
                return null;
            }
            _logger.Log.Info($"Client Credentials GetAccessToken Complete");
            return JsonConvert.SerializeObject(tokenResponse.Json);
        }

    }
}