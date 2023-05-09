using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> ProcessCard([FromBody] string card)
        {
            var randomValue = RandomGen.NextDouble();
            var approved = randomValue > 0.1;
            await Task.Delay(1000);
            Console.WriteLine($"Card {card} processed");
            return Ok(new {Card = card, Approved = approved });
        }
    }
}
