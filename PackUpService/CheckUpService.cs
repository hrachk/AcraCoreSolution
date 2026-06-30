using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AcraData.Data;
using AcraUtils;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CheckUpService.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using AcraUtils.Configuration;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace CheckUpService
{
    public class CheckUpService
    {
        private Logger _logger;               
        private PackUpConfig _configuration;

        public CheckUpService(Logger logger, IOptions<PackUpConfig> configuration)
        {            
            _configuration = configuration.Value;
            _logger = logger;
        }
      
        public async Task<Response> Upload(IFormFile compressedExcelFile)
        {
            var excelFile = DecompressStream(compressedExcelFile);

            Response response = GetErrorResponse(string.Empty);
            try
            {               
                foreach (var path in _configuration.Destination)
                {
                    var filePath = Path.Combine(path, compressedExcelFile.FileName);
                    File.WriteAllBytes(filePath, excelFile);
                    //using (var fileStream = new FileStream(filePath, FileMode.Create))
                    //{
                    //    await compressedExcelFile.CopyToAsync(fileStream);
                    //}
                }
                response.ErrorCode = 200;                
            }
            catch (Exception ex)
            {
                response.ErrorCode = 206;
                response.ErrorDesc = ex.Message; /* Wrong SourceName */
                _logger.Log.Error($"File: {compressedExcelFile.FileName} upload fail Exception: {ex.Message}");
            }
            response.ResponseTime = DateTime.Now.Ticks;
            _logger.Log.Info($"File: {compressedExcelFile.FileName} Uploaded");
            return response;
        }
       
        public static byte[] DecompressStream(IFormFile file)
        {
            using (var sourceStream = file.OpenReadStream())
            {
                using (var outStream = new MemoryStream())
                {
                    AcraUtils.Cryptor cryptor = new Cryptor();
                    byte[] source = cryptor.DecryptDES(ReadFully(sourceStream));

                    using (var gzip = new GZipStream( new MemoryStream(source), CompressionMode.Decompress))
                    {
                        gzip.CopyTo(outStream);
                    }
                    return outStream.ToArray();
                }
            }
        }

        private static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public Response GetErrorResponse(string errorDesc)
        {
            return new Response() { ErrorCode = 206, ResponseTime = DateTime.Now.Ticks,ErrorDesc = errorDesc };
        }        

        private void DeleteExcelFile(string fileName)
        {
            foreach (var path in _configuration.Destination)            
            {                
                File.Delete(Path.Combine(path, fileName));
            }
        }

    }
}
