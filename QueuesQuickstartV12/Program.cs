using Azure.Identity;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using dotnetcore.keyvault.fetch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace QueuesQuickstartV12
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

            // Send several messages to the queue
            await queueClient.SendMessageAsync("First message");
            await queueClient.SendMessageAsync("Second message");

            // Save the receipt so we can update this message later
            SendReceipt receipt = await queueClient.SendMessageAsync("Third message");

            Console.WriteLine("\nPeek at the messages in the queue...");

            // Peek at messages in the queue
            PeekedMessage[] peekedMessages = await queueClient.PeekMessagesAsync(maxMessages: 10);

            foreach (PeekedMessage peekedMessage in peekedMessages)
            {
                // Display the message
                Console.WriteLine($"Message: {peekedMessage.MessageText}");
            }

            Console.WriteLine("\nUpdating the third message in the queue...");

            // Update a message using the saved receipt from sending the message
            await queueClient.UpdateMessageAsync(receipt.MessageId, receipt.PopReceipt, "Third message has been updated");

            Console.WriteLine("\nReceiving messages from the queue...");

            // Get messages from the queue
            QueueMessage[] messages = await queueClient.ReceiveMessagesAsync(maxMessages: 10);

            Console.WriteLine("\nPress Enter key to 'process' messages and delete them from the queue...");
            Console.ReadLine();

            // Process and delete messages from the queue
            foreach (QueueMessage message in messages)
            {
                // "Process" the message
                Console.WriteLine($"Message: {message.MessageText}");

                // Let the service know we're finished with
                // the message and it can be safely deleted.
                await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            }

        }
 
    }
}
