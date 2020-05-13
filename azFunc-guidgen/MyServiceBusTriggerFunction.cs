using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using dotnetcore.azFunction.AppShim;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace azFunc_guidgen
{
    public class MyServiceBusTriggerFunction
    {
        private readonly IFunctionsAppShim _functionsAppShim;
        private readonly ISerializer _serializer;
        private readonly HttpClient _client;

        public MyServiceBusTriggerFunction(
            IFunctionsAppShim functionsAppShim, 
            ISerializer serializer, 
            HttpClient httpClient)
        {
            _functionsAppShim = functionsAppShim;
            _serializer = serializer; 
            _client = httpClient;
        }

        [FunctionName("MyServiceBusTriggerFunction")]
        public async Task Run(
            Microsoft.Azure.WebJobs.ExecutionContext context,
            [ServiceBusTrigger("sbq-queueflow", Connection = "ServiceBusConnection")]string myQueueItem, 
            ILogger logger)
        {
            if (!Global.Initialized)
            {
                await _functionsAppShim.Initialize(logger);
                Global.Initialized = true;
            }
            var job = _serializer.Deserialize<Job>(myQueueItem);
            var json = _serializer.Serialize(job);
            logger.LogInformation($"C# ServiceBus queue trigger function processed message: {json}");


            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, "http://localhost/api/MyServiceBusTriggerFunction");
            httpRequestMessage.Content = new StringContent(
                json,
                Encoding.UTF8, "application/json");

 
            var response = await _functionsAppShim.SendAsync(context,httpRequestMessage);
        }
    }
}
