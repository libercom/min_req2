using Microsoft.AspNetCore.Mvc;

namespace Producer
{
    [ApiController]
    [Route("api")]
    public class BaseController : Controller
    {
        [HttpPost("send")]
        public IActionResult Index(Product product)
        {
            Console.WriteLine($"Your product order was confirmed. Product ID: {product.Id}.");

            return Ok(product);
        }
    }
}
