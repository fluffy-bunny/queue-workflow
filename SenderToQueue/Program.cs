using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Contracts;
using dotnetcore.keyvault.fetch;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SenderToQueue
{
    class Program
    {
     
        static async Task Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole()
                    .AddEventLog();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();

            var connectionStringKeyVaultFetchStore = new SimpleStringKeyVaultFetchStore(
                                new KeyVaultFetchStoreOptions<string>()
                                {
                                    ExpirationSeconds = 3600,
                                    KeyVaultName = "kv-queueflow",
                                    SecretName = "stazfuncqueueflow-primary-connection-string"
                                }, logger);

            var connectionString = await connectionStringKeyVaultFetchStore.GetStringValueAsync();



            var accountName = "stazfuncqueueflow";
            var queueName = "queue-main";

            string queueUri = $"https://{accountName}.queue.core.windows.net/{queueName}";

            // Get a credential and create a client object for the blob container.
            //           QueueClient queueClient = new QueueClient(new Uri(queueUri),new DefaultAzureCredential());
            QueueClient queueClient = new QueueClient(connectionString, queueName);
            // Create the queue
            await queueClient.CreateAsync();

            Console.WriteLine("\nAdding messages to the queue...");

            int index = 0;
            while(true)
            {
                // Send several messages to the queue
                for(int i = 0; i<10; i++)
                {
                    var job = new Job
                    {
                        Id = Guid.NewGuid().ToString(),
                        IssuedTime = DateTime.UtcNow,
                        Name = "My SuperDuper Job"
                    };

                    var encodedMsg = job.Base64Encode();
                    await queueClient.SendMessageAsync(encodedMsg);
                    Console.WriteLine(encodedMsg);
                    index++;
                }
                Thread.Sleep(1000);
            }

        }
    }
}
