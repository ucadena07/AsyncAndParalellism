using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GreetingsController : ControllerBase
    {
        [HttpGet("{name}")]
        public async Task<ActionResult<string>> GetGreetings(string name)
        {
            var waitingTime = RandomGen.NextDouble() * 10 + 1;
            await Task.Delay(TimeSpan.FromSeconds(waitingTime));    
            return $"Hello {name}";
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<string>> GetGoodbye(string name)
        {
            var waitingTime = RandomGen.NextDouble() * 10 + 1;
            await Task.Delay(TimeSpan.FromSeconds(waitingTime));
            return $"GoodBye {name}";
        }
    }
}
