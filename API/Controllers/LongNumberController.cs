using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongNumberController(ILogger<LongNumberController> logger, IUnifiedGamesService<int> uniServ) : ControllerBase
    {
        private readonly ILogger<LongNumberController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IUnifiedGamesService<int> _uniServ = uniServ ?? throw new ArgumentNullException(nameof(uniServ));

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            try
            {
                var sequence = _uniServ.GenerateSequence(level);
                int timeLimit = 3 + level - 1;
                return Ok(new { Sequence = sequence, TimeLimit = timeLimit });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sequence for level {Level}.", level);
                return BadRequest(new { ex.Message });
            }
        }
    }
}
