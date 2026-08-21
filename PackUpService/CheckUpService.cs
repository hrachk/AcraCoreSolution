using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using AcraUtils;
using AcraUtils.Configuration;
using CheckUpService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CheckUpService
{
    public class CheckUpService
    {
        private readonly Logger _logger;
        private readonly PackUpConfig _configuration;

        public CheckUpService(Logger logger, IOptions<PackUpConfig> configuration)
        {
            _configuration = configuration.Value;
            _logger = logger;
        }

        public async Task<Response> Upload(IFormFile compressedExcelFile)
        {
            Response response = GetErrorResponse(string.Empty);

            if (compressedExcelFile == null || compressedExcelFile.Length == 0)
            {
                response.ErrorCode = 206;
                response.ErrorDesc = "File is null or empty";
                response.ResponseTime = DateTime.Now.Ticks;
                _logger.Log.Error("Upload failed: file is null or empty");
                return response;
            }

            if (_configuration.Destination == null || _configuration.Destination.Length == 0)
            {
                response.ErrorCode = 206;
                response.ErrorDesc = "Upload destination is not configured";
                response.ResponseTime = DateTime.Now.Ticks;
                _logger.Log.Error("Upload failed: Destination paths are not configured");
                return response;
            }

            try
            {
                var excelFile = DecompressStream(compressedExcelFile);

                foreach (var path in _configuration.Destination)
                {
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    var filePath = Path.Combine(path, compressedExcelFile.FileName);
                    await File.WriteAllBytesAsync(filePath, excelFile);
                }

                response.ErrorCode = 200;
                response.ErrorDesc = "OK";
                _logger.Log.Info($"File: {compressedExcelFile.FileName} Uploaded successfully");
            }
            catch (Exception ex)
            {
                response.ErrorCode = 206;
                response.ErrorDesc = ex.Message;
                _logger.Log.Error($"File: {compressedExcelFile.FileName} upload fail Exception: {ex.Message}");
            }

            response.ResponseTime = DateTime.Now.Ticks;
            return response;
        }

        public static byte[] DecompressStream(IFormFile file)
        {
            using (var sourceStream = file.OpenReadStream())
            {
                using (var outStream = new MemoryStream())
                {
                    var cryptor = new Cryptor();
                    byte[] source = cryptor.DecryptDES(ReadFully(sourceStream));

                    using (var gzip = new GZipStream(new MemoryStream(source), CompressionMode.Decompress))
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
            using (var ms = new MemoryStream())
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
            return new Response()
            {
                ErrorCode = 206,
                ResponseTime = DateTime.Now.Ticks,
                ErrorDesc = errorDesc
            };
        }

        private void DeleteExcelFile(string fileName)
        {
            if (_configuration.Destination == null)
                return;

            foreach (var path in _configuration.Destination)
            {
                try
                {
                    var fullPath = Path.Combine(path, fileName);
                    if (File.Exists(fullPath))
                        File.Delete(fullPath);
                }
                catch
                {
                    // best-effort cleanup
                }
            }
        }
    }
}
