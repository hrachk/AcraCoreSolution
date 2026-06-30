using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AcraIdentityServer
{
    public class X509CertificateSecretValidator : ISecretValidator
    {
        public Task<SecretValidationResult> ValidateAsync(IEnumerable<Secret> secrets, ParsedSecret parsedSecret)
        {
            var fail = Task.FromResult(new SecretValidationResult { Success = false });
            var success = Task.FromResult(new SecretValidationResult { Success = true });

            if (parsedSecret.Type != "X509Certificate")
            {
                return fail;
            }

            var cert = parsedSecret.Credential as X509Certificate2;

            if (cert == null)
            {
                return fail;
                throw new ArgumentException("ParsedSecret.Credential is not an X509 Certificate");
            }

            string thumbprint = cert.Thumbprint;

            if (string.IsNullOrWhiteSpace(thumbprint))
            {
                return fail;
                throw new ArgumentException("Thumbprint is empty");
            }

            foreach (var secret in secrets)
            {
                if (secret.Type == IdentityServerConstants.SecretTypes.X509CertificateThumbprint)
                {
                    if (TimeConstantComparer.IsEqual(thumbprint.ToLowerInvariant(), secret.Value.ToLowerInvariant()))
                    {
                        return success;
                    }
                }
            }

            return fail;
        }
    }
}
