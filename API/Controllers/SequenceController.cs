using API.Models;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SequenceController(ILogger<SequenceController> logger, IUnifiedGamesService<Square> uniServ) : ControllerBase
    {
        private readonly ILogger<SequenceController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IUnifiedGamesService<Square> _uniServ = uniServ ?? throw new ArgumentNullException(nameof(uniServ));

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            try
            {
                var sequence = _uniServ.GenerateSequence(level);
                return Ok(new { Sequence = sequence, Level = level });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating sequence for level {Level}.", level);
                return BadRequest(new { ex.Message });
            }
        }
    }

}