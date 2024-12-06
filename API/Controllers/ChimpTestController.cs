using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using API.Models;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChimpTestController : ControllerBase
    {
        
        private readonly ILogger<ChimpTestController> _logger;

        public ChimpTestController(ILogger<ChimpTestController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("generate-coordinates")]
        public IActionResult GenerateCoordinates([FromQuery] int level)
        {
            if (level <= 0)
            {
                _logger.LogWarning("Invalid level for chimp test: {Level}", level);
                return BadRequest(new { Message = "Level must be greater than zero." });
            }

            try
            {
                var random = new Random();
                var coordinates = Enumerable.Range(0, level).Select(i => new
                {
                    Number = i + 1,
                    X = random.Next(0, 8),  // board width
                    Y = random.Next(0, 5)   // board height
                }).ToList();

                return Ok(coordinates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating coordinates for chimp test.");
                return StatusCode(500, new { Message = "An error occurred while generating coordinates." });
            }
        }
    }
}
