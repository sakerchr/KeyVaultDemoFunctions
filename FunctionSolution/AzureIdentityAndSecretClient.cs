using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace KeyVaultDemoFunctions
{
    public class AzureIdentityAndSecretClient
    {
        [Function("AzureIdentityAndSecretClient")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "GET")] HttpRequestData req)
        {
            var secret = Environment.GetEnvironmentVariable("mySecret");

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(secret);

            return response;
        }
    }
}
