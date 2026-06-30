using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcraIdentityServer
{
    public class X509CertificateSecretParser : ISecretParser
    {
        private readonly ILogger _logger;
        private readonly IdentityServerOptions _options;

        public X509CertificateSecretParser(IdentityServerOptions options, ILogger<X509CertificateSecretParser> logger)
        {
            _options = options;
            _logger = logger;
        }

        public string AuthenticationMethod => "ClientCertificate";

        public Task<ParsedSecret> ParseAsync(HttpContext context)
        {
            _logger.LogDebug("Start parsing for X.509 certificate");

            var certificate = context.Connection.ClientCertificate;

            if (certificate == null)
            {
                _logger.LogDebug("Client certificate is null");
                return Task.FromResult<ParsedSecret>(null);
            }
            
            if (!context.Request.HasFormContentType)
            {
                _logger.LogDebug("Content type is not a form");
                return Task.FromResult<ParsedSecret>(null);
            }

            var body = context.Request.Form;

            if (body == null)
            {
                _logger.LogDebug("No form found");
                return Task.FromResult<ParsedSecret>(null);
            }

            // If a client Id is not provided, then use serial number as id
            var id = body["client_id"].FirstOrDefault() ?? certificate.SerialNumber;

            if (string.IsNullOrWhiteSpace(id))
            {
                _logger.LogDebug("No client id found");
                return Task.FromResult<ParsedSecret>(null);
            }

            if (id.Length > _options.InputLengthRestrictions.ClientId)
            {
                _logger.LogError("Client ID exceeds maximum lenght.");
                return Task.FromResult<ParsedSecret>(null);
            }

            return Task.FromResult(new ParsedSecret
            {
                Id = id,
                Type = "X509Certificate",
                Credential = certificate
            });
        }
    }
}