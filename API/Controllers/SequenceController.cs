using System;
using API.Models;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SequenceController : ControllerBase
    {
        private readonly ILogger<SequenceController> _logger;
        private readonly IUnifiedGamesService<Square> _uniServ;

        public SequenceController(ILogger<SequenceController> logger, IUnifiedGamesService<Square> uniServ)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _uniServ = uniServ ?? throw new ArgumentNullException(nameof(uniServ));
        }

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            if (level <= 0)
            {
                _logger.LogWarning("Level {Level} is invalid. Must be greater than zero.", level);
                return BadRequest(new { Message = "Level must be greater than zero." });
            }

            try
            {
                var sequence = _uniServ.GenerateSequence(this, level);

                if (sequence == null || sequence.Length == 0)
                {
                    _logger.LogError("Service returned an invalid sequence for level {Level}", level);
                    return StatusCode(500, new { Message = "Failed to generate a valid sequence." });
                }

                return Ok(new { Sequence = sequence, Level = level });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                _logger.LogWarning(ex, "Invalid level provided: {Level}", level);
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Failed to generate sequence for level {Level}", level);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while generating sequence for level {Level}", level);
                return StatusCode(500, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
    }
}