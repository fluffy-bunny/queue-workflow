using Contracts;
using MediatR;
using Microsoft.Azure.ServiceBus;
using ServiceBusCLI.Features.GenerateSecurityAccessSignature;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using ServiceBusCLI.Utils;

namespace ServiceBusCLI.Features.SendJob
{
    public static class SendJob
    {
        public class Request : IRequest<Response>
        {
            public QueueClient QueueClient { get; internal set; }
            public string Message { get; internal set; }
 
            public Request(IQueueClientAccessor queueClientAccessor)
            {
                QueueClient = queueClientAccessor.QueueClient;
            }
        }

        public class Response
        {
            public string Result { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public async Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                try
                {
                    byte[] byteMessage = Encoding.UTF8.GetBytes(request.Message);
                    await request.QueueClient.SendAsync(new Message(byteMessage));
                    return await RespondWith(request.Message);
                }
                catch(Exception ex)
                {
                    return await RespondWith(ex.Message);
                }
               
            }

            private Task<Response> RespondWith(string result)
            {
                return Task.FromResult(new Response { Result = result });
            }
        }
    }
}
