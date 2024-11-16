using Microsoft.AspNetCore.Mvc;
using API.Interfaces;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SequenceController(ISequenceService service, IUnifiedGamesService<Square> uni) : ControllerBase
    {
        private readonly ISequenceService _service = service ?? throw new ArgumentNullException(nameof(service));
        private readonly IUnifiedGamesService<Square> _uni = uni;

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            var sequence = _uni.GenerateSequence(_service, level);
            return Ok(new { Sequence = sequence, Level = level });
        }
    }
}