using Microsoft.AspNetCore.Mvc;

namespace BackRoll.Telegram.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("ok");
        }
    }
}
