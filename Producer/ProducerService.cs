using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json.Serialization;

namespace Producer
{
    public class ProducerService
    {
        private readonly IList<Product> _availableProducts = new List<Product>();

        public ProducerService()
        {
            Start();
        }

        public async Task Start()
        {
            await Task.Delay(5000);

            await GetAvailableProducts();

            for (int i = 0; i < 5; i++)
            {
                Task.Run(Generate);
            }
        }

        private async Task Generate()
        {
            while (true)
            {
                await Task.Delay(3000);

                var index = Random.Shared.Next(0, _availableProducts.Count);

                var httpClient = new HttpClient();
                var serializedValue = JsonConvert.SerializeObject(_availableProducts[index]);

                await httpClient.PostAsync("http://aggregator:80/api/order", new StringContent(serializedValue, Encoding.UTF8, "application/json"));
            }
        }

        private async Task GetAvailableProducts()
        {
            var httpClient = new HttpClient();

            var result = await httpClient.GetAsync("http://aggregator:80/api");
            var data = await result.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<List<Guid>>(data);

            foreach (var item in products)
            {
                _availableProducts.Add(new Product { Id = item });
            }
        }
    }
}
