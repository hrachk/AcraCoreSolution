using AcraIDServices.Models.AVV;
using AcraUtils;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;


namespace AcraIDServices
{
    public class AVVClient
    {

        private Logger _logger;
        AcraUtils.Configuration.AVVConfig _AVVConfiguration;
      
        private HttpResponseMessage response;
        private HttpRequestMessage request;
        private string result;
        private dynamic data;

        public HttpResponseMessage Response
        {
            get { return response; }
        }

        public HttpRequestMessage Request
        {
            get { return request; }
        }

        public string Result
        {
            get { return result; }
        }

        public dynamic Data
        {
            get { return data; }
        }

        public AVVClient(Logger logger, IOptions<AcraUtils.Configuration.AVVConfig> configuration)
        {
            _logger = logger;
            if (configuration?.Value == null)
            {
                _logger.Log.Error("AVVConfig is null – check appsettings AVVConfiguration section");
                throw new ArgumentNullException(nameof(configuration), "AVVConfig is required");
            }
            _AVVConfiguration = configuration.Value;
        }

        public async void GetPersonData(RequestType requestType, object Item)
        {           
            try
            {
                response = new HttpResponseMessage();
                request = new HttpRequestMessage();

                var searchValues = new Dictionary<string, string>();           

                var url = _AVVConfiguration.URL;
                switch (requestType)
                {
                    case RequestType.SSN:
                        searchValues.Add("psn", ((Models.AVV.BySSN)Item).psn);
                        searchValues.Add("Addresses", (((Models.AVV.BySSN)Item).Addresses == null)?Models.AVV.Addresses.CURRENT.ToString(): ((Models.AVV.BySSN)Item).Addresses.ToString());
                        break;
                    case RequestType.Document:                        
                        searchValues.Add("docnum", ((Models.AVV.ByDocument)Item).docnum);
                        searchValues.Add("Addresses", (((Models.AVV.ByDocument)Item).Addresses == null) ? Models.AVV.Addresses.CURRENT.ToString() : ((Models.AVV.ByDocument)Item).Addresses.ToString());
                        break;
                    case RequestType.Name:                        
                        searchValues.Add("first_name", ((Models.AVV.ByName)Item).first_name);
                        searchValues.Add("last_name", ((Models.AVV.ByName)Item).last_name);
                        searchValues.Add("middle_name", ((Models.AVV.ByName)Item).middle_name);
                        searchValues.Add("birth_date", ((Models.AVV.ByName)Item).birth_date);
                        searchValues.Add("Addresses", (((Models.AVV.ByName)Item).Addresses == null) ? Models.AVV.Addresses.CURRENT.ToString() : ((Models.AVV.ByName)Item).Addresses.ToString());
                        break;
                    default:
                        break;
                }


                _logger.Log.InfoFormat("Requesting[{0}][]: {1}", requestType, searchValues);



                var client = new HttpClient();
                
                request = new HttpRequestMessage(HttpMethod.Post, url) { Content = new FormUrlEncodedContent(searchValues) };
                response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    result = response.Content.ReadAsStringAsync().Result;
                    logRequest(searchValues, response, result);

                    AvvResponse aVVResponse = JsonConvert.DeserializeObject<AvvResponse>(result);                    
                    if (aVVResponse.Status == "ok")
                    {
                        data = aVVResponse;
                    }
                    else
                    {
                        //Error
                        data = null;
                        result = aVVResponse.Message;
                    }
                }              

            }
            catch (Exception ex)
            {
                _logger.Log.Error("Request ID: " + requestType + " Error: " + ex.Message);
                result = "Request Error";                
            }            
        }

        private void logRequest(Dictionary<string, string> searchValues, dynamic response, dynamic result)
        {

            if (_AVVConfiguration.debug == true)
            {
                using (StreamWriter sw = System.IO.File.AppendText("EkengLog.txt"))
                {
                    DateTime localDate = DateTime.Now;

                    sw.WriteLine("--------------------------------------");
                    sw.WriteLine(System.Text.Json.JsonSerializer.Serialize(localDate).ToString());
                    sw.WriteLine(System.Text.Json.JsonSerializer.Serialize(searchValues).ToString());
                    sw.WriteLine(System.Text.Json.JsonSerializer.Serialize(response).ToString());
                    sw.WriteLine(System.Text.Json.JsonSerializer.Serialize(result).ToString());
                    sw.WriteLine("--------------------------------------");

                }

            }
        }

        public enum RequestType
        {
            SSN,
            Document,
            Name
        }        
    }
}
