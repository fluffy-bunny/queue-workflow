using AutoMapper;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusCLI.Features.GenerateSecurityAccessSignature
{
    /*
     *       "commandLineArgs": "generate-sas -k 8u4ZWemBetr**REDACTED**b/7IEOP3/c= -p RootManageSharedAccessKey -e 3600"
     */

    public static class Commands
    {
        [Command("generate-sas", Description = "generate a SecurityAccessSignature")]
        public class GenerateSecurityAccessSignatureCommand
        {
            [Option("-k|--key", CommandOptionType.SingleValue, Description = "ServiceBus Secret Key, i.e. 8u4ZWemBetr9WdRDsnGpBjbC79Vk0MTRLb/7IEOP3/c=")]
            public string Key { get; set; }

            [Option("-p|--policy", CommandOptionType.SingleValue, Description = "i.e. RootManageSharedAccessKey")]
            public string Policy { get; set; }

            [Option("-e|--expiry", CommandOptionType.SingleValue, Description = "in seconds")]
            public int ExpirySeconds { get; set; }

            private async Task OnExecuteAsync(
                IMediator mediator, 
                IMapper mapper, 
                IConsole console, 
                IFooService fs,
                GenerateSecurityAccessSignature.Request request)
            {
                var command = mapper.Map(this, request);

                var response = await mediator.Send(command);

                console.WriteLine($"{response.Result}");
            }
        }
    }
}
