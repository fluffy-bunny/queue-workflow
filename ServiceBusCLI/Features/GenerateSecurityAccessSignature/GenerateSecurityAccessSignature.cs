﻿using MediatR;
using ServiceBusCLI.Features.ServiceBus;
using ServiceBusCLI.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusCLI.Features.GenerateSecurityAccessSignature
{
    public static class GenerateSecurityAccessSignature
    {
        public static string SettingsFileName = "GenerateSecurityAccessSignature.json";
        public class Settings
        {
            public string Token { get; set; }
        }
        public class Request : IRequest<Response>
        {
            public AppSettings<ServiceBus.ServiceBusSettings.Settings> AppSettings { get; }
            public AppSettings<Settings> SasSettings { get; }

            public Request(
                AppSettings<ServiceBus.ServiceBusSettings.Settings> appsettings,
                AppSettings<GenerateSecurityAccessSignature.Settings> sasSettings)
            {
                AppSettings = appsettings;
                SasSettings = sasSettings;

            }
            public string Key { get; set; }

            public string Policy { get; set; }

            public int ExpirySeconds { get; set; }
            public bool Session { get; set; }
        }

        public class Response
        {
            public string Result { get; set; }
        }

        public class Handler : IRequestHandler<Request, Response>
        {
            public Task<Response> Handle(Request request, CancellationToken cancellationToken)
            {
                var settings = request.AppSettings.Load("service-bus-queue-settings.json");
                if (settings == null)
                {
                    return RespondWith($"appsettings is null, did you forget to call service-bus-settings");
                }


                var sasToken = SeviceBusSecurityAccessSignatureGenerator
                    .GenerateSecurityAccessSignature(settings.Namespace, settings.Queue, request.Key, request.Policy, 
                    new TimeSpan(0, 0, request.ExpirySeconds));

                if (request.Session)
                {
                    var sasSettings = new Settings
                    {
                        Token = sasToken
                    };
                    request.SasSettings.Save(sasSettings, SettingsFileName);
                    sasSettings = request.SasSettings.Load(SettingsFileName);
                }
                

                return RespondWith(sasToken);
            }

            private Task<Response> RespondWith(string result)
            {
                return Task.FromResult(new Response { Result = result });
            }
        }
    }
}
