using Microsoft.AspNetCore.Mvc;

namespace Aggregator
{
    [ApiController]
    [Route("api")]
    public class BaseController : Controller
    {
        private readonly AggregatorService _aggregatorService;

        public BaseController(AggregatorService aggregatorService)
        {
            _aggregatorService = aggregatorService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _aggregatorService.GetAllProducts();

            return Ok(products);
        }

        [HttpPost("order")]
        public IActionResult Order(Product product)
        {
            Console.WriteLine($"Somebody ordered: {product.Id}");

            _aggregatorService.AggregateFromProducer(product);

            return Ok();
        }

        [HttpPost("confirm")]
        public IActionResult Confirm(Product product)
        {
            Console.WriteLine($"Recieved a confirmation for product {product.Id}");

            _aggregatorService.AggregateFromConsumer(product);

            return Ok();
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterModel registerModel)
        {
            Console.WriteLine($"Registering {registerModel.Endpoint}");

            _aggregatorService.Register(registerModel);

            return Ok();
        }
    }
}
