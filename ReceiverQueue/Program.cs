using Azure.Identity;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Contracts;
using dotnetcore.keyvault.fetch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ReceiverQueue
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


            while (true)
            {
                QueueMessage[] messages = await queueClient.ReceiveMessagesAsync(maxMessages: 10);

                // Process and delete messages from the queue
                foreach (QueueMessage message in messages)
                {
                    var decoded = message.MessageText.Base64Decode();
                    // "Process" the message
                    Console.WriteLine($"Message: {message.MessageText} - {decoded}");
                    var job = message.MessageText.Base64Decode<Job>();

                    // Let the service know we're finished with
                    // the message and it can be safely deleted.
                    await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
                }

                Thread.Sleep(1000);
            }

        }
    }
}
