using Microsoft.AspNetCore.Mvc;
using API.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserScoreController(UserScoreService userScoreService) : ControllerBase
    {
        private readonly UserScoreService _userScoreService = userScoreService;

        // Fetch leaderboard
        [HttpGet("leaderboard/{gameType}")]
        public IActionResult GetLeaderboard(string gameType, int top = 10)
        {
            try
            {
                var leaderboard = _userScoreService.GetLeaderboard()
                                .Where(score => score.GameType == gameType)
                                .OrderByDescending(score => score.Score)
                                .Take(top)
                                .ToList();

                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while fetching the leaderboard:" + ex);
            }
        }
    }
}
