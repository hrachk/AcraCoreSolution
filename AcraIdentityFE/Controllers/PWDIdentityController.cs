using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AcraIdentityFE.Models;
using AcraUtils;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;
using RestSharp;
using System.IO;

namespace AcraIdentityFE.Controllers
{
    public class PWDIdentityController : Controller
    {
        private Logger _logger;        
        private AcraUtils.Configuration.AcraIdentityConfig _configuration;

        public PWDIdentityController(Logger logger, IOptions<AcraUtils.Configuration.AcraIdentityConfig> configuration)
        {            
            _configuration = configuration.Value;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        /*public string GetAccessToken([FromBody] string data)
        {
            AcraUtils.Cryptor cryptor = new Cryptor();
            var loginParams = JsonConvert.DeserializeObject<LoginModel>(cryptor.DecryptDES(data));
          
            HttpClient client = new HttpClient();
            var tokenResponse = client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = $"{_configuration.IdentityServerUrl}",
                ClientId = loginParams.ClientID,
                ClientSecret = loginParams.ClientSecret,
                UserName = loginParams.Username,
                Password = loginParams.Password,
                Scope = loginParams.Scope,
                GrantType = "password"
            }).Result;

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                _logger.Log.Error($"Password Credentials GetAccessToken Failed Error: {tokenResponse.Error}");
                //  return null;
            }
            _logger.Log.Info($"Password Credentials GetAccessToken Complete");
            return JsonConvert.SerializeObject(tokenResponse.Json);
        }*/

        [HttpGet]
        public string GetAccessToken(string Username, string Password, string ClientID, string ClientSecret, string Scope)
        {
            //System.IO.File.AppendAllText("C:/Logs/IdentityFE.txt", $"{DateTime.Now} GetAcessToken Started" + Environment.NewLine);
            // It's Work
            //var client = new RestClient($"{_configuration.IdentityServerUrl}");

            //var request = new RestRequest(Method.POST);

            //request.AddHeader("cache-control", "no-cache");

            //request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            //request.AddParameter("undefined", "grant_type=password&client_id=CheckUpClient&password=123456&client_secret=c7b4e734-2788-45be-94f5-905711f8d243&username=a&undefined=", ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);

            //return response.Content;

            AcraUtils.Cryptor cryptor = new Cryptor();

            Username = cryptor.DecryptDES(Username);
            Password = cryptor.DecryptDES(Password);
            ClientID = cryptor.DecryptDES(ClientID);
            ClientSecret =RemoveWhitespace(cryptor.DecryptDES(ClientSecret));
            Scope = cryptor.DecryptDES(Scope);

            HttpClient client = new HttpClient();
            var tokenResponse = client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {                
                Address = $"{_configuration.IdentityServerUrl}",
                ClientId = ClientID,
                ClientSecret = ClientSecret,
                UserName = Username,
                Password = Password,
                Scope = Scope,
                GrantType = "password"
            }).Result;
            //System.IO.File.AppendAllText("C:/Logs/IdentityFE.txt", $"{DateTime.Now} Token Response Done" + Environment.NewLine);


            if (tokenResponse.IsError)
            {
                //System.IO.File.AppendAllText("C:/Logs/IdentityFE.txt", $"{DateTime.Now} Token Response Error" + Environment.NewLine);

                Console.WriteLine(tokenResponse.Error);
                _logger.Log.Error($"Password Credentials GetAccessToken Failed Error: {tokenResponse.Error}");
              //  return null;
            }
            _logger.Log.Info($"Password Credentials GetAccessToken Complete");
            //System.IO.File.AppendAllText("C:/Logs/IdentityFE.txt", $"{DateTime.Now} GetAcessTokenCompleate" + Environment.NewLine);
            return JsonSerializer.Serialize(tokenResponse.Json);
            //return JsonConvert.SerializeObject(tokenResponse.Json);
        }
        public static string RemoveWhitespace( string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

    }
}