using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GreetingsController : ControllerBase
    {
        [HttpGet("{name}")]
        public ActionResult<string> GetGreetings(string name)
        {
            return $"Hello {name}";
        }
    }
}
