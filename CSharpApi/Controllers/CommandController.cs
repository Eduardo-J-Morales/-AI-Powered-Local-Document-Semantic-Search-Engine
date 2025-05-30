using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CommandExecutor.Models;

namespace CommandExecutor.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CommandController : ControllerBase
    {
        private readonly ILogger<CommandController> _logger;

        public CommandController(ILogger<CommandController> logger)
        {
            _logger = logger;
        }

        [HttpPost("execute")]
        public async Task<IActionResult>
    }
}