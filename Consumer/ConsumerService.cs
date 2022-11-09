using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;

namespace Consumer
{
    public class ConsumerService
    {
        private readonly ConcurrentQueue<Product> _consumerShared = new ConcurrentQueue<Product>();
        private readonly List<Product> _products = new List<Product>();

        private readonly string _endpoint;

        public ConsumerService(IConfiguration configuration)
        {
            _endpoint = configuration.GetSection("Endpoint").Value;

            Start();
        }

        public void Consume(Product product)
        {
            _consumerShared.Enqueue(product);
        }

        private async Task Start()
        {
            for (int i = 0; i < 10; i++)
            {
                _products.Add(new Product() { Id = Guid.NewGuid() });
            }

            await Task.Delay(3000);

            await Register();

            Console.WriteLine($"Register me {_endpoint}");

            for (int i = 0; i < 3; i++)
            {
                Task.Run(Send);
            }
        }

        private async Task Send()
        {
            while (true)
            {
                if (!_consumerShared.IsEmpty)
                {
                    await Task.Delay(3000);

                    _consumerShared.TryDequeue(out var value);

                    var httpClient = new HttpClient();
                    var serializedValue = JsonConvert.SerializeObject(value);

                    await httpClient.PostAsync("http://aggregator:80/api/confirm", new StringContent(serializedValue, Encoding.UTF8, "application/json"));
                }
            }
        }

        private async Task Register()
        {
            var registerModel = new RegisterModel
            {
                Endpoint = _endpoint,
                Products = _products
            };

            var httpClient = new HttpClient();
            var serializedValue = JsonConvert.SerializeObject(registerModel);

            await httpClient.PostAsync("http://aggregator:80/api/register", new StringContent(serializedValue, Encoding.UTF8, "application/json"));
        }
    }
}
