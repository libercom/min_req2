using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;

namespace Aggregator
{
    public class AggregatorService
    {
        private readonly ConcurrentQueue<Product> _producerShared = new ConcurrentQueue<Product>();
        private readonly ConcurrentQueue<Product> _consumerShared = new ConcurrentQueue<Product>();

        private readonly Dictionary<Guid, string> _registredConsumers = new Dictionary<Guid, string>();

        public AggregatorService()
        {
            Start();
        }

        public List<Guid> GetAllProducts()
        {
            var products = _registredConsumers.Keys.ToList();

            return products;
        }

        public void Register(RegisterModel registerModel)
        {
            foreach (var item in registerModel.Products)
            {
                _registredConsumers.Add(item.Id, registerModel.Endpoint);
            }
        }

        public void AggregateFromConsumer(Product product)
        {
            _consumerShared.Enqueue(product);
        }

        public void AggregateFromProducer(Product product)
        {
            _producerShared.Enqueue(product);
        }

        private async Task Start()
        {

            for (int i = 0; i < 3; i++)
            {
                Task.Run(SendToConsumer);
            }

            for (int i = 0; i < 3; i++)
            {
                Task.Run(SendToProducer);
            }
        }

        private async Task SendToConsumer()
        {
            while (true)
            {
                if (!_producerShared.IsEmpty)
                {
                    await Task.Delay(3000);

                    _producerShared.TryDequeue(out var value);

                    var httpClient = new HttpClient();
                    var serializedValue = JsonConvert.SerializeObject(value);
                    
                    await httpClient.PostAsync(_registredConsumers[value.Id], new StringContent(serializedValue, Encoding.UTF8, "application/json"));
                }
            }
        }

        private async Task SendToProducer()
        {
            while (true)
            {
                if (!_consumerShared.IsEmpty)
                {
                    await Task.Delay(3000);

                    _consumerShared.TryDequeue(out var value);

                    var httpClient = new HttpClient();
                    var serializedValue = JsonConvert.SerializeObject(value);

                    await httpClient.PostAsync("http://producer:80/api/send", new StringContent(serializedValue, Encoding.UTF8, "application/json"));
                }
            }
        }
    }
}
