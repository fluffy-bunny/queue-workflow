using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace dotnetcore.azFunction.AppShim
{
    public delegate void LoadConfigurationsDelegate(IConfigurationBuilder config, string environmentName);
    public interface IFunctionsAppShim
    {
        Task Initialize(ExecutionContext context, ILogger log);
        Task<HttpResponseMessage> Run(
            Microsoft.Azure.WebJobs.ExecutionContext context,
            HttpRequest request);
        Task<HttpResponseMessage> SendAsync(Microsoft.Azure.WebJobs.ExecutionContext context, 
            HttpRequestMessage httpRequestMessage);
    }
}
