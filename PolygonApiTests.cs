using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace stock_trade
{
    public class PolygonApiTests
    {
        AlpacaConfig config;
        public PolygonApiTests()
        {
            config = JsonConvert.DeserializeObject<AlpacaConfig>(File.ReadAllText(@"AlpacaConfig.json"));
        }

        private async Task Test1()
        {
            var client = new HttpClient();
            var requestMessage = new HttpRequestMessage();
            requestMessage.Method = HttpMethod.Get;
            requestMessage.RequestUri = new Uri($"https://api.polygon.io/v2/aggs/ticker/AAPL/range/1/day/2019-01-01/2019-02-01?apiKey={config.KeyId}");
            var res = await client.SendAsync(requestMessage);
            var content = await res.Content.ReadAsStringAsync();
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
                Console.WriteLine(content);
            else
                Console.WriteLine($"HttpStatusCode: {res.StatusCode}");
        }

        /// <summary>
        /// Daily Open/Close
        /// </summary>
        /// <returns></returns>
        private async Task Test2()
        {
            var client = new HttpClient();
            var requestMessage = new HttpRequestMessage();
            requestMessage.Method = HttpMethod.Get;
            requestMessage.RequestUri = new Uri($"https://api.polygon.io/v1/open-close/AAPL/2021-01-08?apiKey={config.KeyId}");
            var res = await client.SendAsync(requestMessage);
            var content = await res.Content.ReadAsStringAsync();
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
                Console.WriteLine(content);
            else
                Console.WriteLine($"HttpStatusCode: {res.StatusCode}");
        }

        public void Run()
        {
            Task.Run(() => Test2());
        }
    }
}
