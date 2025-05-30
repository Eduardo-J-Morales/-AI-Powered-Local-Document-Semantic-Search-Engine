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
        public async Task<IActionResult> ExecuteCommand([FromBody] CommandRequest request)
        {
            if (!IsCommandAllowed(request.Command)) return BadRequest("Command not permitted")

            try
            {
                var result = await RunProcessAsync(request.Command, request.Arguments);
                return Ok(new { Output = result });
            }
            catch (Exception e) {
                _logger.LogError(exception, "Command execution fialed");
                return StatusCode(500, "Execution error");
            }
        }

        private bool IsCommandAllowed(string command) {
            var allowedCommands = new[] { "dir", "ls", "echo", "pwd" };
            return allowedCommands.Contains(command.ToLower());
        }

        private async Task<string> RunProcessAsync(string command, string arguments) {
            var processInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
        }
    }
}