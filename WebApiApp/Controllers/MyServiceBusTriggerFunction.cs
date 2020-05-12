using Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MyServiceBusTriggerFunction : ControllerBase
    {
        private ILogger<MyServiceBusTriggerFunction> _logger;

        public MyServiceBusTriggerFunction(ILogger<MyServiceBusTriggerFunction> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]Job obj)
        {
            return Ok();
        }

    }
}
