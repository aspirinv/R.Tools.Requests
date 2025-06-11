using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace R.Tools.Requests.Sample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private static List<Product> data = [
            new Product(1,"Gruntmaster 6000", "A new product less advanced than the Gruntmaster 9000 but just as fun", 700),
            new Product(2,"The Hitchhiker's Guide to the Galaxy", "It has the words DON'T PANIC inscribed in large friendly letters on its cover", 42)
            ];

        [HttpGet]
        [SkipRequestSaving]
        public ActionResult<IEnumerable<Product>> Get()
        {
            return Ok(data);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Post(Product p)
        {
            data.Add(p with { id = data.Count + 1});
            return Ok();
        }
    }

    public record Product(int id, string name, string description, decimal price);
}
