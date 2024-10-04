using Microsoft.AspNetCore.Mvc;
using API.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserScoreController : ControllerBase
    {
        private readonly UserScoreService _userScoreService;

        public UserScoreController(UserScoreService userScoreService)
        {
            _userScoreService = userScoreService;
        }

        // Fetch leaderboard
        [HttpGet("leaderboard")]
        public async Task<IActionResult> GetLeaderboard(int top = 10) 
        {
            try {
                var leaderboard = (await _userScoreService.GetLeaderboard())
                                .OrderByDescending(score => score.Score)
                                .Take(top)
                                .ToList();

                return Ok(leaderboard);
            } catch (Exception ex) {
                return StatusCode(500, "An error occurred while fetching the leaderboard:" + ex);
            }
        }
    }
}
