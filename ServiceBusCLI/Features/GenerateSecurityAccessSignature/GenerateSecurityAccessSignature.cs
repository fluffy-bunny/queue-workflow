using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusCLI.Features.GenerateSecurityAccessSignature
{
    public static class GenerateSecurityAccessSignature
    {
        public class Request : IRequest<Response>
        {
            public string Key { get; set; }

            public string Policy { get; set; }

            public int ExpirySeconds { get; set; }
        }

        public class Response
        {
            public string Result { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var sasToken = SeviceBusSecurityAccessSignatureGenerator
                    .GenerateSecurityAccessSignature(Program.Namespace, Program.Queue, request.Key, request.Policy, 
                    new TimeSpan(0, 0, request.ExpirySeconds));
                return RespondWith(sasToken);
            }

            private Task<Response> RespondWith(string result)
            {
                return Task.FromResult(new Response { Result = result });
            }
        }
    }
}
