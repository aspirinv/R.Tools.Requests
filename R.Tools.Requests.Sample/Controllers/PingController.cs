using Microsoft.AspNetCore.Mvc;

namespace R.Tools.Requests.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private static Random rnd = new Random();

        [HttpPost]
        public ActionResult Ping()
        {
            if (rnd.Next(10) == 5)
                return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: "Internal failure", title: "ERROR");
            return Ok();
        }
    }
}
