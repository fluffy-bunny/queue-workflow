using Contracts.Extensions;
using dotnetcore.azFunction.AppShim;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(azFunc_guidgen.Startup))]
namespace azFunc_guidgen
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();
            builder.Services.AddSerializer();
            builder.Services.AddBase64Encoder();
            var functionsAppShim = new FunctionsAppShim<WebApiApp.Startup>
            {
                LoadConfigurationsDelegate = WebApiApp.Program.LoadConfigurations
            };
            builder.Services.AddSingleton<IFunctionsAppShim>(functionsAppShim);
        }
    }
}
