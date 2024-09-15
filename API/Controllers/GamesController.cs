using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("games")]
    public class GamesController : ControllerBase
    {
        private static List<string> games = new List<string>
        {
            "Long Number Memory", "Chimp Test", "Sequence Memorization"
        };

        [HttpGet]
        public IActionResult GetGames()
        {
            return Ok(games);
        }

        [HttpPost]
        public IActionResult AddGame([FromBody] string gameName)
        {
            if (string.IsNullOrWhiteSpace(gameName))
            {
                return BadRequest("Game name cannot be empty.");
            }

            games.Add(gameName);
            return CreatedAtAction(nameof(GetGames), new { gameName }, gameName);
        }
    }
}
