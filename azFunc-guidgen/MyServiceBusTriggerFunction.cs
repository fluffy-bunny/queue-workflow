using System;
using System.Net.Http;
using System.Threading.Tasks;
using Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace azFunc_guidgen
{
    public class MyServiceBusTriggerFunction
    {
        private readonly ISerializer _serializer;
        private readonly HttpClient _client;

        public MyServiceBusTriggerFunction(ISerializer serializer, HttpClient httpClient)
        {
            _serializer = serializer; 
            _client = httpClient;
        }

        [FunctionName("MyServiceBusTriggerFunction")]
        public async Task Run(
            [ServiceBusTrigger("sbq-queueflow", Connection = "ServiceBusConnection")]
            string myQueueItem, 
            ILogger log)
        {
            var job = _serializer.Deserialize<Job>(myQueueItem);
            var json = _serializer.Serialize(job);
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {json}");
        }
    }
}
