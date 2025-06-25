namespace Cachara.Playground;

public class AlgoTrading
{
    private static void Test(string[] args)
    {
        var simulator = new MarketSimulator();
        var prices = simulator.GeneratePrices(200);

        var trader = new MovingAverageTrader(5, 20);
        trader.Run(prices);

        Console.WriteLine("\n--- Trade History ---");
        foreach (var trade in trader.TradeHistory)
        {
            Console.WriteLine(trade);
        }
    }
}

public class PriceData
{
    public DateTime Timestamp { get; set; }
    public decimal ClosePrice { get; set; }

    public override string ToString()
    {
        return $"{Timestamp:yyyy-MM-dd} | {ClosePrice:C}";
    }
}

public readonly struct Trade(DateTime timestamp, decimal price, string action)
{
    public override string ToString()
    {
        return $"[{timestamp:yyyy-MM-dd}] {action} @ {price:C}";
    }
}

public class MarketSimulator
{
    public List<PriceData> GeneratePrices(int count)
    {
        var rng = new Random();
        var prices = new List<PriceData>();
        var price = 100m;

        for (var i = 0; i < count; i++)
        {
            price += (decimal)(rng.NextDouble() - 0.5); // small fluctuations
            prices.Add(new PriceData { Timestamp = DateTime.Today.AddDays(i), ClosePrice = price });
        }

        return prices;
    }
}

public class MovingAverageTrader
{
    private readonly int _longPeriod;
    private readonly int _shortPeriod;
    private bool _inPosition;

    public MovingAverageTrader(int shortPeriod, int longPeriod)
    {
        _shortPeriod = shortPeriod;
        _longPeriod = longPeriod;
    }

    public List<Trade> TradeHistory { get; } = new();

    public void Run(List<PriceData> prices)
    {
        for (var i = _longPeriod; i < prices.Count; i++)
        {
            var shortSMA = prices.Skip(i - _shortPeriod).Take(_shortPeriod).Average(p => p.ClosePrice);
            var longSMA = prices.Skip(i - _longPeriod).Take(_longPeriod).Average(p => p.ClosePrice);
            var currentPrice = prices[i];

            if (shortSMA > longSMA && !_inPosition)
            {
                TradeHistory.Add(new Trade(currentPrice.Timestamp, currentPrice.ClosePrice, "Buy"));
                _inPosition = true;
            }
            else if (shortSMA < longSMA && _inPosition)
            {
                TradeHistory.Add(new Trade(currentPrice.Timestamp, currentPrice.ClosePrice, "Sell"));
                _inPosition = false;
            }
        }
    }
}
