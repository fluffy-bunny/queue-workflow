using AutoMapper;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusCLI.Features.ServiceBus
{
 

    public static class Commands
    {
        [Command("service-bus-settings", Description = "set service-bus queue settings for the cli")]
        public class ServiceBusSettingsCommand
        {
            [Option("-n|--namespace", CommandOptionType.SingleValue, Description = "ServiceBus Namespace")]
            public string Namespace { get; set; }

            [Option("-q|--queue", CommandOptionType.SingleValue, Description = "Queue Name")]
            public string Queue { get; set; }

            private async Task OnExecuteAsync(
                IMediator mediator,
                IMapper mapper,
                IConsole console,
                IFooService fs,
                ServiceBusSettings.Request request)
            {
                var command = mapper.Map(this, request);

                var response = await mediator.Send(command);

                console.WriteLine($"{response.Result}");
            }
        }
    }
}
