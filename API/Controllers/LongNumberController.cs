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
