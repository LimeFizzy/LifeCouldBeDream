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
                var allCoordinates = new List<(int X, int Y)>();
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 5; y++)
                    {
                        allCoordinates.Add((x, y));
                    }
                }

                var random = new Random();
                var shuffled = allCoordinates.OrderBy(_ => random.Next()).ToList();

                var coordinates = shuffled.Take(level)
                                          .Select((coord, index) => new {
                                              Number = index + 1,
                                              X = coord.X,
                                              Y = coord.Y
                                          })
                                          .ToList();

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
