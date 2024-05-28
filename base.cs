using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Helper;

namespace YourNamespace
{
    public static class ReadRecords
    {
        [FunctionName("ReadRecordsFunction")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var helper = new Helper();

            log.LogInformation("C# HTTP trigger function processed a request.");
            var KeyVault = new KeyVault(new Uri(helper.KeyVaultUrl));
            var service = new ServiceClient(KeyVault.connectionString);

            var query = new QueryExpression("entityname");
            var records = service.RetrieveMultiple(query);

            string responseMessage = string.Empty;

            foreach (var record in records.Entities)
            {
                responseMessage += record.ToString() + "\n";
            }

            return new OkObjectResult(responseMessage);
        }
    }

    public class KeyVault
    {
        public var connectionString { get; }
        public new KeyVault(Uri kvUri)
        {
            var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());

            connectionString = $"AuthType=OAuth;Username={client.GetSecret("username").Value.Value}; Password={client.GetSecret("password").Value.Value}; Url={client.GetSecret("url").Value.Value}; AppId={client.GetSecret("appId").Value.Value}; RedirectUri={client.GetSecret("redirectUri").Value.Value};";
        }
    }
}