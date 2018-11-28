using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace VSTSBuildAgent
{
    public static class VSTSBuildAgent
    {
        private static IAzure _azure = null;

        [FunctionName("StartVSTSBuildAgent")]
        public static async Task<HttpResponseMessage> StartVSTSBuildAgentAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var rg = await AzureConn.ResourceGroups.GetByNameAsync(ConfigurationManager.AppSettings["resourceGroupName"]);
            var agentName = await GetNameAsync(req, "agentName");
            var env = new Dictionary<string, string>
            {
                { "VSTS_AGENT_INPUT_URL", ConfigurationManager.AppSettings["VSTS_AGENT_INPUT_URL"] },
                { "VSTS_ACCOUNT", ConfigurationManager.AppSettings["VSTS_ACCOUNT"] },
                { "VSTS_AGENT_INPUT_AUTH", "pat" },
                { "VSTS_AGENT_INPUT_TOKEN", ConfigurationManager.AppSettings["VSTS_AGENT_INPUT_TOKEN"] },
                { "VSTS_TOKEN", ConfigurationManager.AppSettings["VSTS_AGENT_INPUT_TOKEN"] },
                { "VSTS_AGENT_INPUT_POOL", "ACIPool" },
                { "VSTS_POOL", "ACIPool" },
                { "VSTS_AGENT_INPUT_AGENT", agentName },
                { "VSTS_AGENT", agentName }
            };

            var containerGroup = await AzureConn.ContainerGroups.Define(agentName)
                .WithRegion(rg.RegionName)
                .WithExistingResourceGroup(rg)
                .WithLinux()
                .WithPublicImageRegistryOnly()
                .WithoutVolume()
                .DefineContainerInstance(agentName)
                    .WithImage(ConfigurationManager.AppSettings["imageName"])
                    .WithoutPorts()
                    .WithEnvironmentVariables(env)
                    .Attach()
                .CreateAsync();

            return req.CreateResponse(HttpStatusCode.OK, "VSTS agent is running");
        }

        [FunctionName("StopVSTSBuildAgent")]
        public static async Task<HttpResponseMessage> StopVSTSBuildAgenttAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var agentName = await GetNameAsync(req, "agentName");
            await AzureConn.ContainerGroups.DeleteByResourceGroupAsync(ConfigurationManager.AppSettings["resourceGroupName"], agentName);
            return req.CreateResponse(HttpStatusCode.OK, "VSTS agent has been removed");
        }

        private static async Task<string> GetNameAsync(HttpRequestMessage req, string key)
        {
            // parse query parameter
            var name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Equals(q.Key, key, StringComparison.OrdinalIgnoreCase))
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            return (name ?? (string)data?.name).ToLower();
        }
        private static IAzure AzureConn
        {
            get
            {
                if (_azure == null)
                { 
                    var tenantId = ConfigurationManager.AppSettings["tenantId"];
                    var sp = new ServicePrincipalLoginInformation
                    {
                        ClientId = ConfigurationManager.AppSettings["clientId"],
                        ClientSecret = ConfigurationManager.AppSettings["clientSecret"]
                    };
                    _azure = Azure.Authenticate(new AzureCredentials(sp, tenantId, AzureEnvironment.AzureGlobalCloud)).WithDefaultSubscription();
                }
                return _azure;
            }
        }
    }
}
