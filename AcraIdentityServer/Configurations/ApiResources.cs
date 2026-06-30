using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcraIdentityServer.Configurations
{
    public class ApiResources
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("trigger", "Trigger Web Service Poll"),
                new ApiResource("ATM", "ATM Service"),
                new ApiResource("CheckUp", "CheckUp Excel Files Upload Service"),
            };
        }

    }
}
