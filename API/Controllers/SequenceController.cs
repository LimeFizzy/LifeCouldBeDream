using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using System;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SequenceController : ControllerBase
    {
        private readonly ISequenceService _service;

        public SequenceController(ISequenceService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            if (level <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Level must be greater than zero.");
            }

            try
            {
                var sequence = _service.GenerateSequence(level);
                if (sequence == null || sequence.Length == 0)
                {
                    throw new InvalidOperationException("Failed to generate a valid sequence.");
                }

                return Ok(new { Sequence = sequence, Level = level });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Error = ex.Message });
            }
        }
    }
}
