using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Models;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SequenceController : ControllerBase
    {
        private readonly SequenceService _service;

        public SequenceController(SequenceService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet("generate-sequence/{level}")]
        public IActionResult GenerateSequence(int level)
        {
            var sequence = _service.GenerateSequence(level);
            return Ok(new { Sequence = sequence, Level = level });
        }

        // [HttpPost("submit-score")]
        // public IActionResult SubmitScore([FromBody] ScoreSubmission scoreSubmission)
        // {
        //     int score = _service.CalculateScore(
        //         scoreSubmission.Level, 
        //         scoreSubmission.CorrectSequence, 
        //         scoreSubmission.GuessedSequence
        //     );

        //     if (score == 0)
        //     {
        //         return BadRequest(new { Message = "Incorrect sequence." });
        //     }

        //     return Ok(new { Message = "Score submitted successfully.", Score = score });
        // }
    }
}