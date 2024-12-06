using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using API.Models;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChimpTestController(ILogger<ChimpTestController> logger, IUnifiedGamesService<SquareChimp> uniServ) : ControllerBase
    {

        private readonly ILogger<ChimpTestController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IUnifiedGamesService<SquareChimp> _uniServ = uniServ ?? throw new ArgumentNullException(nameof(uniServ));
        

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            try
            {
                var random = new Random();
                var coordinates = Enumerable.Range(0, level).Select(i => new
                {
                    Number = i + 1,
                    X = random.Next(0, 8),
                    Y = random.Next(0, 5)
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
