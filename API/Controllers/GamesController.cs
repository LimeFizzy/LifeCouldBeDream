using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("games")]
    public class GamesController : ControllerBase
    {
        private static readonly List<GameDTO> games =
        [
            new GameDTO
            {
                GameID = 1,
                Title = "Long number memory",
                Description = "Memorize as many digits of a long number as possible.",
                Icon = "src/assets/longNumberIcon.svg",
                AltText = "Long number memory icon",
                Route = "/longNumber",
            },
            new GameDTO
            {
                GameID = 2,
                Title = "Chimp Test",
                Description = "Test your memory by remembering the order of numbers displayed on the screen.",
                Icon = "src/assets/chimpIcon.svg",
                AltText = "Chimp test icon",
                Route = "/chimpTest",
            },
            new GameDTO
            {
                GameID = 3,
                Title = "Sequence Memorization",
                Description = "Remember and recall increasingly larger sequence of action showed.",
                Icon = "src/assets/sequenceIcon.svg",
                AltText = "Sequence memory icon",
                Route = "/sequence",
            }
        ];


        [HttpGet]
        public ActionResult<IEnumerable<GameDTO>> GetGames()
        {
            return Ok(games);
        }

        [HttpPost]
        public IActionResult AddGame([FromBody] GameDTO newGame)
        {
            if (newGame == null || string.IsNullOrWhiteSpace(newGame.Title))
            {
                return BadRequest("Game details cannot be empty.");
            }

            newGame.GameID = games.Max(g => g.GameID) + 1;
            games.Add(newGame); 

            return CreatedAtAction(nameof(GetGames), new { id = newGame.GameID }, newGame);
        }
    }
}
