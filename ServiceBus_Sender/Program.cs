using Contracts;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBus_Sender
{
    class Program
    {
        static string Namespace = "sb-queueflow";
        static string Queue = "sbq-queueflow";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var queueUri = $"sb://{Namespace}.servicebus.windows.net/";

            var tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
            QueueClient sendClient = new QueueClient(queueUri, Queue, tokenProvider);
            var job = new Job
            {
                Id = Guid.NewGuid().ToString(),
                IssuedTime = DateTime.UtcNow,
                Name = "My SuperDuper Job"
            };
            byte[] byteMessage = Encoding.UTF8.GetBytes(job.Base64Encode());
//            byte[] byteMessage = job.ToByteArray();
            await sendClient.SendAsync(new Message(byteMessage));
            await sendClient.CloseAsync();

        }
    }
}
