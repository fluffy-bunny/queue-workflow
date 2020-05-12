using ConsoleUtils;
using Contracts;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus_Sender
{
    class Program
    {
        static string Namespace = "sb-queueflow";
        static string Queue = "sbq-queueflow";
        static ISerializer _serializer;
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var queueUri = $"sb://{Namespace}.servicebus.windows.net/";

            _serializer = new Serializer();
            var tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
            QueueClient sendClient = new QueueClient(queueUri, Queue, tokenProvider);
            while (true)
            {
                for(int i = 0; i < 10; i++)
                {
                    var job = new Job
                    {
                        Id = Guid.NewGuid().ToString(),
                        IssuedTime = DateTime.UtcNow,
                        Name = "My SuperDuper Job"
                    };
                    var json = _serializer.Serialize(job);
                    Console.WriteLine(json);
                    byte[] byteMessage = Encoding.UTF8.GetBytes(json);
                    await sendClient.SendAsync(new Message(byteMessage));
                }

                if (ConsoleHelper.QuitRequest(2000))
                {
                    break;
                }
            }
            
            await sendClient.CloseAsync();

        }
       
    }
}
