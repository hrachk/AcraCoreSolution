using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;

namespace AcraIdentityServer.Configurations
{
    public class Clients
    {
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                new Client
                {
                    ClientId = "ExpSignal_TestUser",
                    AllowedGrantTypes = new List<string> { "X509Certificate", GrantType.ClientCredentials },

                    ClientSecrets =
                    {
                        new Secret("SssINvUTRLe0k3tBh7NHB2c28Ep0FhX4SqUXNvq5qNs="),//password.Sha256()
                        new Secret
                        {
                            Value = "c2260dff11957102308bf629fea66fa0c7cc3f1f",//cert Thumbprint
                            Type = IdentityServerConstants.SecretTypes.X509CertificateThumbprint
                        }
                    },
                    AllowedScopes = { "trigger" }
                },
                new Client
                {
                    ClientId = "00FEE8509AB9A4185E",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret
                        {
                            Value = "c2260dff11957102308bf629fea66fa0c7cc3f1f",//cert Thumbprint
                            Type = IdentityServerConstants.SecretTypes.X509CertificateThumbprint
                        }
                    },
                    AllowedGrantTypes = new List<string> { "X509Certificate", GrantType.ClientCredentials },

                    AllowedScopes = new List<string>
                    {
                        "trigger"
                    }
                },
                new Client
                {

                    ClientId = "ATMClient",
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret("5ea379d6-5547-451e-9389-3401aa0c4a74".Sha256())
                    },
                    AllowedScopes = new List<string>
                    {
                        "ATM"
                    },
                    AllowedGrantTypes = new List<string> { "X509Certificate", GrantType.ClientCredentials }                    
                },
                new Client()
                {
                    ClientId = "CheckUpClient",
                    ClientSecrets = new List<Secret>()
                    {
                        new Secret("c7b4e734-2788-45be-94f5-905711f8d243".Sha256())
                    },
                    AllowedScopes =  new List<string>()
                    {
                        "CheckUp"
                    },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword
                }
            };
        }
    }
}
