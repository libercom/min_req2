using Microsoft.AspNetCore.Mvc;

namespace Consumer
{
    [ApiController]
    [Route("api")]
    public class BaseController : Controller
    {
        private readonly ConsumerService _consumerService;

        public BaseController(ConsumerService consumerService)
        {
            _consumerService = consumerService;
        }

        [HttpPost("send")]
        public IActionResult Send(Product product)
        {
            Console.WriteLine($"Your product order was confirmed. Product ID {product.Id}");
            _consumerService.Consume(product);

            return Ok();
        }
    }
}
