using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using API.Extensions;
using API.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongNumberController(ILongNumberService service) : ControllerBase
    {
        private readonly ILongNumberService _service = service;

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
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
                return BadRequest("Could not generate a valid sequence after multiple attempts.");
            }

            int timeLimit = 3 + level - 1;

            return Ok(new { Sequence = sequence, TimeLimit = timeLimit });
        }

    }
}
