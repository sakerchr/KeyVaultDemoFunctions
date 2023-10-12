using System.Net;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace KeyVaultDemoFunctions
{
    public class AzureAppServiceReferences
    {
        [Function("AzureAppServiceReferences")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "GET")] HttpRequestData req)
        {
            var identityCredential = new DefaultAzureCredential();

            var vaultUri = new Uri("https://myvault.vault.azure.net/");
            var secretClient = new SecretClient(vaultUri, identityCredential);

            var secretName = "mySecret";
            var secret = await secretClient.GetSecretAsync(secretName);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(secret.Value);

            return response;
        }
    }
}
