using System;
using System.Net.Http;
using System.Threading.Tasks;
using Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace azFunc_guidgen
{
    public class MyQueueTriggerFunction
    {
        private readonly ISerializer _serializer;
        private readonly HttpClient _client;

        public MyQueueTriggerFunction(ISerializer serializer, HttpClient httpClient)
        {
            _serializer = serializer;
            _client = httpClient;
        }

        [FunctionName("MyQueueTriggerFunction")]
        public async Task Run([QueueTrigger("queue-main", Connection = "ConnectionStringStorageAccount")]string myQueueItem, 
            ILogger log)
        {
            var job = _serializer.Deserialize<Job>(myQueueItem);
            var json = _serializer.Serialize(job);
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {json}");

            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
