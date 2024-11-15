using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using API.Data;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // Correct namespace for ILogger

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GamesController> _logger;

        public GamesController(AppDbContext context, ILogger<GamesController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDTO>>> GetGames()
        {
            try
            {
                var games = await _context.Games.ToListAsync();
                if (games == null || games.Count == 0)
                {
                    _logger.LogWarning("No games found in the database.");
                    return NotFound(new { Message = "No games found in the database." });
                }
                return Ok(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching games.");
                return StatusCode(500, new { Message = "An error occurred while fetching games.", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddGame([FromBody] GameDTO newGame)
        {
            if (newGame == null || string.IsNullOrWhiteSpace(newGame.Title))
            {
                _logger.LogWarning("Game details cannot be empty or missing a title.");
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
                _logger.LogError(ex, "An error occurred while saving the game to the database.");
                return StatusCode(500, new { Message = "An error occurred while saving the game to the database.", Error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding the game.");
                return StatusCode(500, new { Message = "An unexpected error occurred while adding the game.", Error = ex.Message });
            }
        }
    }
}
