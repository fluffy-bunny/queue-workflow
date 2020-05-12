using System;
using System.Net.Http;
using System.Threading.Tasks;
using Contracts;
using dotnetcore.azFunction.AppShim;
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
        private IFunctionsAppShim _functionsAppShim;
        public MyQueueTriggerFunction(IFunctionsAppShim functionsAppShim,ISerializer serializer, HttpClient httpClient)
        {
            _serializer = serializer;
            _client = httpClient;
            _functionsAppShim = functionsAppShim;
        }
       
        [FunctionName("MyQueueTriggerFunction")]
        public async Task Run([QueueTrigger("queue-main", Connection = "ConnectionStringStorageAccount")]string myQueueItem, 
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

            logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
           
        }
    }
}
