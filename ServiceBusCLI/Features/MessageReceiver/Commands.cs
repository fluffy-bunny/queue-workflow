using AutoMapper;
using Contracts;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using Microsoft.Azure.ServiceBus;
using ServiceBusCLI.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusCLI.Features.MessageReceiver
{
    public static class Commands
    {

        [Command("receive", Description = "Receive Message")]
        public class ReceiveCommand
        {
            [Option("-c|--complete", CommandOptionType.NoValue, Description = "complete the message")]
            public bool Complete { get; } = false;
            private async Task OnExecuteAsync(
                IMediator mediator,
                IMapper mapper,
                IConsole console,
                IMessageReceiverAccessor messageReceiverAccessor,
                ISerializer serializer,
                Receive.Request request)
            {
                if (messageReceiverAccessor.MessageReceiver == null)
                {
                    console.WriteLine($"QueueClient is null, did you forget to call service-bus-settings");
                    return;
                }
                var command = mapper.Map(this, request);
                console.WriteLine($"Sending Request....");
                var response = await mediator.Send(command);
                if (response.Exception != null)
                {
                    console.WriteLine($"{response.Exception.Message}");
                }
                else
                {
                    if(response.Result == null)
                    {
                        console.WriteLine($"Success: No Messages Available......");
                    }
                    else
                    {
                        var json = Encoding.UTF8.GetString(response.Result.Body);
                        var job = serializer.Deserialize<Job>(json);
                        json = job.ToJson();
                        console.WriteLine(json);
                    }
                }
            }
        }

        [Command("peek", Description = "peek")]
        public class PeekCommand
        {
            private async Task OnExecuteAsync(
                IMediator mediator,
                IMapper mapper,
                IConsole console,
                IMessageReceiverAccessor messageReceiverAccessor,
                ISerializer serializer,
                Peek.Request request)
            {
                if (messageReceiverAccessor.MessageReceiver == null)
                {
                    console.WriteLine($"QueueClient is null, did you forget to call service-bus-settings");
                    return;
                }
                var command = mapper.Map(this, request);
                console.WriteLine($"Sending Request....");
                var response = await mediator.Send(command);
                if(response.Exception != null)
                {
                    console.WriteLine($"{response.Exception.Message}");
                }
                else
                {
                    var json = Encoding.UTF8.GetString(response.Result.Body);
                    var job = serializer.Deserialize<Job>(json);
                    json = job.ToJson();
                    console.WriteLine(json);
                }
            }
        }
    }
}
