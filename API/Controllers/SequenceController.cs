using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; // Required for ILogger
using API.Interfaces;
using System;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SequenceController : ControllerBase
    {
        private readonly ISequenceService _service;
        private readonly ILogger<SequenceController> _logger;

        public SequenceController(ISequenceService service, ILogger<SequenceController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            _logger.LogInformation("Received request to generate sequence for level {Level}", level);

            if (level <= 0)
            {
                _logger.LogWarning("Level {Level} is invalid. Must be greater than zero.", level);
                return BadRequest(new { Message = "Level must be greater than zero." });
            }

            try
            {
                _logger.LogDebug("Calling service to generate sequence for level {Level}", level);

                var sequence = _service.GenerateSequence(level);

                if (sequence == null || sequence.Length == 0)
                {
                    _logger.LogError("Service returned an invalid sequence for level {Level}", level);
                    return StatusCode(500, new { Message = "Failed to generate a valid sequence." });
                }

                _logger.LogInformation("Successfully generated sequence for level {Level}", level);
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
