using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using API.Extensions;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongNumberController : ControllerBase
    {
        private readonly LongNumberService _service;

        public LongNumberController(LongNumberService service)
        {
            _service = service;
        }

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            var sequence = _service.GenerateSequence(level);
            while (!sequence.IsValidSequence(level))
            {
                sequence = _service.GenerateSequence(level);
            }
            int timeLimit = 3 + level - 1;

            return Ok(new { Sequence = sequence, TimeLimit = timeLimit });
        }

    }
}
