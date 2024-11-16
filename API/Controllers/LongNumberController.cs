using System;
using API.Services;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongNumberController : ControllerBase
    {
        private readonly ILongNumberService _service;
        private readonly ILogger<LongNumberController> _logger;
        private readonly IUnifiedGamesService<int> _uniServ;

        public LongNumberController(ILongNumberService service, ILogger<LongNumberController> logger, IUnifiedGamesService<int> uniServ)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uniServ = uniServ ?? throw new ArgumentNullException(nameof(uniServ));
        }

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            if (level <= 0)
            {
                _logger.LogWarning("Invalid level {Level}. Level must be greater than zero.", level);
                return BadRequest(new { Message = "Level must be greater than zero." });
            }

            const int maxRetries = 10;
            int retries = 0;

            try
            {
                var sequence = _uniServ.GenerateSequence(_service, level);

                while (!sequence.IsValidSequence(level) && retries < maxRetries)
                {
                    sequence = _uniServ.GenerateSequence(_service, level);
                    retries++;
                }

                if (retries == maxRetries)
                {
                    _logger.LogError("Failed to generate a valid sequence for level {Level} after {MaxRetries} attempts.", level, maxRetries);
                    return BadRequest(new { Message = "Could not generate a valid sequence after multiple attempts." });
                }

                int timeLimit = 3 + level - 1;
                return Ok(new { Sequence = sequence, TimeLimit = timeLimit });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while generating a sequence for level {Level}.", level);
                return StatusCode(500, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
    }
}
