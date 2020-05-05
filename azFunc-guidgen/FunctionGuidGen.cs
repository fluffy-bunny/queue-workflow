using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace azFunc_guidgen
{
    public partial class GuidGenResponse
    {
        [JsonProperty("guid")]
        public Guid Guid { get; set; }
        [JsonProperty("creationTime")]
        public DateTime CreationTime { get; internal set; }
    }
    public static class FunctionGuidGen
    {
        [FunctionName("GuidGen")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string responseMessage = Guid.NewGuid().ToString();
            var response = new GuidGenResponse
            {
                Guid = Guid.NewGuid(),
                CreationTime = DateTime.UtcNow
            };
            return new OkObjectResult(response);
        }
    }
}
