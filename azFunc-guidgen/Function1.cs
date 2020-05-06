using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace azFunc_guidgen
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void Run([QueueTrigger("queue-main", Connection = "ConnectionStringStorageAccount")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
