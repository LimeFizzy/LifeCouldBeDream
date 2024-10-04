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
        // for now let's just take top 10
        public IActionResult GetLeaderboard(int top = 10) 
        {
            var leaderboard = _userScoreService.GetLeaderboard()
                               .OrderByDescending(score => score.Score)
                               .Take(top)
                               .ToList();

            return Ok(leaderboard);
        }
    }
}
