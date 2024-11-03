using Microsoft.AspNetCore.Mvc;
using API.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SequenceController(ISequenceService service) : ControllerBase
    {
        private readonly ISequenceService _service = service ?? throw new ArgumentNullException(nameof(service));

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            var sequence = _service.GenerateSequence(level);
            return Ok(new { Sequence = sequence, Level = level });
        }
    }
}