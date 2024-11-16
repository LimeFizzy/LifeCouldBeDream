using Microsoft.AspNetCore.Mvc;
using API.Models;
using API.Services;
using API.Extensions;
using API.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LongNumberController(ILongNumberService service, IUnifiedGamesService<int> uni) : ControllerBase
    {
        private readonly ILongNumberService _service = service;
        private readonly IUnifiedGamesService<int> _uni = uni;

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            var sequence = _uni.GenerateSequence(_service, level);
            while (!sequence.IsValidSequence(level))
            {
                sequence = _uni.GenerateSequence(_service, level);
            }
            int timeLimit = 3 + level - 1;

            return Ok(new { Sequence = sequence, TimeLimit = timeLimit });
        }

    }
}
