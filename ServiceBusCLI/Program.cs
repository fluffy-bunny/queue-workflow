
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using MediatR;
using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Contracts;
using ServiceBusCLI.Features.SendJob;
using ServiceBusCLI.Features.GenerateSecurityAccessSignature;

namespace ServiceBusCLI
{
    [Command(Name = "SuperTool",
           Description = "Run helpful utilities for my application")]
    [HelpOption]
    [VersionOptionFromMember(MemberName = "GetVersion")]
    [Subcommand(
           typeof(Features.When.Commands.WhenCommand),
           typeof(Features.GenerateSecurityAccessSignature.Commands.GenerateSecurityAccessSignatureCommand),
           typeof(Features.SendJob.Commands.SendJobCommand)
        )
       ]
    internal class Program
    {
        static public string Namespace = "scalesets";
        static public string Queue = "w10rs5pr0";

        private static async Task Main(string[] args)
        {
            await CreateHostBuilder()
                .RunCommandLineApplicationAsync<Program>(args);
        }
        public static IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddMediatR(typeof(Program).Assembly)
                        .AddAutoMapper(typeof(Program).Assembly);
                    services.AddSingleton<IFooService, FooService>();
                    services.AddSingleton(sp =>
                    {
                        var queueUri = $"sb://{Namespace}.servicebus.windows.net/";
                        var tokenProvider = TokenProvider.CreateManagedIdentityTokenProvider();
                        var queueClient = new QueueClient(queueUri, Queue, tokenProvider);
                        return queueClient;
                    });
                    services.AddSingleton<ISerializer, Serializer>();
                    services.AddTransient<SendJob.Request>();
                    services.AddTransient<GenerateSecurityAccessSignature.Request>();
                    
                });
           }
        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.WriteLine("You must specify a command");
            app.ShowHelp();
            return 1;
        }

        private string GetVersion()
        {
            return typeof(Program).Assembly
                ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
        }
    }
}
