using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dotnetcore.azFunction.AppShim
{
    public static class Global
    {
        public static bool Initialized = false;
        public delegate void Initialize();
        private static readonly object startupLock = new object();
        internal static async Task LockWrapperAsync(Initialize func)
        {
            lock (startupLock)
            {
                func();
            }

        }

        public static async Task InitializeShimAsync(Microsoft.Azure.WebJobs.ExecutionContext context, IFunctionsAppShim functionsAppShim, ILogger logger)
        {
            await LockWrapperAsync(async () =>
            {
                if (!Initialized)
                {
                    await functionsAppShim.Initialize(context,logger);
                    Initialized = true;
                }
            });

        }
    }
}
