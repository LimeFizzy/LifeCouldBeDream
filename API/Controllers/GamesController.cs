using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GamesController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetGames()
        {
            try
            {
                var games = await _context.Games.ToListAsync();
                if (games == null || games.Count == 0)
                {
                    return NotFound(new { Message = "No games found in the database." });
                }

                return Ok(games);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred while fetching games.", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddGame([FromBody] GameDTO newGame)
        {
            if (newGame == null || string.IsNullOrWhiteSpace(newGame.Title))
            {
                return BadRequest(new { Message = "Game details cannot be empty or missing a title." });
            }

            try
            {
                _context.Games.Add(newGame);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetGames), new { id = newGame.GameID }, newGame);
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { Message = "An error occurred while saving the game to the database.", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred while adding the game.", Error = ex.Message });
            }
        }
    }
}
