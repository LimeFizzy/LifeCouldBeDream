using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using System;
using API.Extensions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongNumberController : ControllerBase
    {
        private readonly ILongNumberService _service;

        public LongNumberController(ILongNumberService service)
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

            const int maxRetries = 10;
            var sequence = _service.GenerateSequence(level);

            int retries = 0;
            while (!sequence.IsValidSequence(level) && retries < maxRetries)
            {
                sequence = _service.GenerateSequence(level);
                retries++;
            }

            if (retries == maxRetries)
            {
                return BadRequest(new { Message = "Could not generate a valid sequence after multiple attempts." });
            }

            int timeLimit = 3 + level - 1;

            return Ok(new { Sequence = sequence, TimeLimit = timeLimit });
        }
    }
}
