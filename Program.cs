using Alpaca.Markets;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;

namespace stock_trade
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");

            var config = JsonConvert.DeserializeObject<AlpacaConfig>(File.ReadAllText(@"AlpacaConfig.json"));

            var tradingClient = Environments.Paper.GetAlpacaTradingClient(new SecretKey(config.KeyId, config.SecretKey));
            // Paper environment doesn't work for me for Polygon.
            var polygonClient = Environments.Live.GetPolygonDataClient(config.KeyId);
            var polygonStreamingClient = Environments.Live.GetPolygonStreamingClient(config.KeyId);


            var clock = await tradingClient.GetClockAsync();

            if (clock != null)
            {
                Console.WriteLine($"Timestamp: {clock.TimestampUtc}, NextOpen: {clock.NextOpenUtc}, NextClose: {clock.NextCloseUtc}");
            }

            var today = DateTime.Today.AddDays(-2);
            var bars = await polygonClient.ListAggregatesAsync(new AggregatesRequest("AAPL", new AggregationPeriod(1, AggregationPeriodUnit.Minute))
                .SetInclusiveTimeInterval(today, today.AddDays(1)));
            var lastBars = bars.Items.Skip(Math.Max(0, bars.Items.Count() - 20));

            List<Decimal> closingPrices = new List<Decimal>();
            foreach (var bar in lastBars)
            {
                if (bar.TimeUtc?.Date == today)
                {
                    closingPrices.Add(bar.Close);
                }
            }

            polygonStreamingClient.ConnectAndAuthenticateAsync().Wait();

            polygonStreamingClient.QuoteReceived += (quote) =>
            {
                Console.WriteLine(quote.TimeUtc);
            };

            polygonStreamingClient.SubscribeQuote("AAPL");


            var polygonApiTests = new PolygonApiTests();
            polygonApiTests.Run();


            Console.ReadLine();
        }
    }
}
