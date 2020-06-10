using ConsoleUtils;
using Contracts;
using Microsoft.Azure.Management.ServiceBus;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBus_Receiver
{
    class Program
    {
        static string ResourceGroupName = "rg-scalesets";
        static string Namespace = "scalesets";
        static string Queue = "w10rs5pr0";
        static string Key = "8u4ZWemBetr9WcRDsnFpBjaC79Vk0MTTLb/7IEOP3/c=";
        static string Policy = "RootManageSharedAccessKey";
        static QueueClient _queueClient;
        static ISerializer _serializer;

        public static TokenProvider _tokenProvider { get; private set; }
        static string _queueUri { get; set; }
        public static Uri _queueServiceUri { get; private set; }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            

            
            //scalesets.servicebus.windows.net
            _queueUri = $"sb://{Namespace}.servicebus.windows.net/";
            _queueServiceUri = CreateServiceUri(Namespace, Queue);

            _serializer = new Serializer();
            _tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();

            var token = await _tokenProvider.GetTokenAsync(_queueServiceUri.ToString(), new TimeSpan(24,0,0));
            var creds = new TokenCredentials(token.TokenValue);
            var sbClient = new ServiceBusManagementClient(creds)
            {
                SubscriptionId = "39ac48fb-fea0-486a-ba84-e0ae9b06c663"
            }; 
            //var keys = await sbClient.Queues.ListKeysAsync(ResourceGroupName,Namespace,Queue, "RootManageSharedAccessKey");

            var sasToken = SeviceBusSecurityAccessSignatureGenerator.GenerateSecurityAccessSignature(Namespace, Queue, Key, Policy, new TimeSpan(24, 0, 0));
            Console.WriteLine($"SAS: {sasToken}");
            _queueClient = new QueueClient(_queueUri, Queue, _tokenProvider);
            
            Console.WriteLine("===========================================================");
            Console.WriteLine("Press ENTER key to exit after receiving all the messages.");
            Console.WriteLine("===========================================================");

            // Register QueueClient's MessageHandler and receive messages in a loop
            RegisterOnMessageHandlerAndReceiveMessages();

            while (!ConsoleHelper.QuitRequest(2000)){}

            await _queueClient.CloseAsync();
        }
        private static Uri CreateServiceUri(string serviceNamespace, string queuePath)
        {
            return new Uri(string.Format($"https://{serviceNamespace}.servicebus.windows.net/{queuePath}/messages"));
        }

        static void RegisterOnMessageHandlerAndReceiveMessages()
        {
            // Configure the message handler options in terms of exception handling, number of concurrent messages to deliver, etc.
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                // Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
                // Set it according to how many messages the application wants to process in parallel.
                MaxConcurrentCalls = 1,

                // Indicates whether the message pump should automatically complete the messages after returning from user callback.
                // False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
                AutoComplete = false
                
            };

            // Register the function that processes messages.
            _queueClient.RegisterMessageHandler(ProcessMessagesAsync, messageHandlerOptions);

            var sessionHandlerOptions = new SessionHandlerOptions(ExceptionSessionHandler)
            {
                AutoComplete = false,
                MaxConcurrentSessions = 1 
            };

            //_queueClient.RegisterSessionHandler(HandleMessageAsync, sessionHandlerOptions);
        }
        private static async Task HandleMessageAsync(
            IMessageSession session, 
            Message message, CancellationToken token)
        {
            Console.WriteLine($"MessageId:{message.MessageId} SessionId:{session.SessionId}, LockToken:{ message.SystemProperties.LockToken}");
        //    await using var receiver = queueClient.CreateReceiver(scope.QueueName);

//            queueClient.RenewMessageLockAsync(message.SystemProperties.LockToken)
            var sessionClient = new SessionClient(endpoint:_queueUri, Queue, _tokenProvider);

           // var messageSession = await sessionClient.AcceptMessageSessionAsync(session.SessionId);

           // await messageSession.RenewSessionLockAsync();


            var json = Encoding.UTF8.GetString(message.Body);
            var job = _serializer.Deserialize<Job>(json);
            json = job.ToJson();
            await session.RenewLockAsync(message.SystemProperties.LockToken);
            //            await session.RenewSessionLockAsync();
        }

        static Task ExceptionSessionHandler(ExceptionReceivedEventArgs arg)
        {
            Console.WriteLine(arg.Exception.Message);
            return Task.CompletedTask;
        }

        // Use this handler to examine the exceptions received on the message pump.
        static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            Console.WriteLine("Exception context for troubleshooting:");
            Console.WriteLine($"- Endpoint: {context.Endpoint}");
            Console.WriteLine($"- Entity Path: {context.EntityPath}");
            Console.WriteLine($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
        static async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            // Process the message.

            // Deserialize the hashtable from the file and
            // assign the reference to the local variable.

            var json = Encoding.UTF8.GetString(message.Body);
            var job = _serializer.Deserialize<Job>(json);
            json = job.ToJson();

            Console.WriteLine($"Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{json}");
            // Complete the message so that it is not received again.
            // This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
            Console.WriteLine($"{DateTime.UtcNow}");
            Console.WriteLine($"MessageId:{message.MessageId}, IsLockTokenSet:{message.SystemProperties.IsLockTokenSet}, LockToken:{ message.SystemProperties.LockToken}, expiry:{message.SystemProperties.LockedUntilUtc}");
//            await _queueClient.CompleteAsync(message.SystemProperties.LockToken);

            // Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
            // If queueClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
            // to avoid unnecessary exceptions.
        }
    }
}
