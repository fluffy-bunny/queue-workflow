using System;
using Contracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace azFunc_guidgen
{
    public static class MyServiceBusTriggerFunction
    {
        [FunctionName("MyServiceBusTriggerFunction")]
        public static void Run(
            [ServiceBusTrigger("sbq-queueflow", Connection = "ServiceBusConnection")]
            string myQueueItem, 
            ILogger log)
        {
            var job = myQueueItem.Base64Decode<Job>();
            var json = job.ToJson();
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {json}");
        }
    }
}
