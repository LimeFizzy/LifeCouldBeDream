using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetGames()
        {
            var games = await _context.Games.ToListAsync();
            return Ok(games);
        }

        [HttpPost]
        public async Task<IActionResult> AddGame([FromBody] GameDTO newGame)
        {
            if (newGame == null || string.IsNullOrWhiteSpace(newGame.Title))
            {
                return BadRequest("Game details cannot be empty.");
            }

            _context.Games.Add(newGame);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGames), new { id = newGame.GameID }, newGame);
        }
    }
}
