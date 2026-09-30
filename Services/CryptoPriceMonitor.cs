using Microsoft.Toolkit.Uwp.Notifications;
using SmartCubeMobile.MockData;
using System.Globalization;

namespace SmartCubeMobile.Services
{
    public static class CryptoPriceMonitor
    {
        private static Timer _timer;
        private static readonly HashSet<string> _notified = new();
        private static readonly CultureInfo _gbp = new("en-GB");
        private static bool _running;

        public static void Start()
        {
            if (_running) return;
            _running = true;
            _timer = new Timer(_ => _ = CheckAlertsAsync(), null,
                TimeSpan.FromSeconds(15), TimeSpan.FromMinutes(3));   // CoinGecko's free limit is tight
        }

        public static void Stop()
        {
            _running = false;
            _timer?.Dispose();
            _timer = null;
        }

        private static async Task CheckAlertsAsync()
        {
            try
            {
                var alerts = MockDataService.GetPriceAlerts().ToList();
                if (alerts.Count == 0) return;

                var coins = await CryptoMarketService.GetTop50Async();
                if (coins == null || coins.Count == 0) return;

                var lookup = coins.ToDictionary(c => c.Symbol, c => c);

                foreach (var alert in alerts)
                {
                    if (!lookup.TryGetValue(alert.Symbol, out var coin)) continue;

                    var triggered = alert.Direction == "Above"
                        ? coin.PriceGBP >= alert.TargetPrice
                        : coin.PriceGBP <= alert.TargetPrice;

                    var key = $"{alert.Symbol}_{alert.Direction}_{alert.TargetPrice}";

                    if (triggered && !_notified.Contains(key))
                    {
                        _notified.Add(key);
                        alert.IsTriggered = true;
                        ShowToast(alert, coin.PriceGBP);
                    }
                    else if (!triggered)
                    {
                        _notified.Remove(key);
                    }
                }
            }
            catch { }
        }

        private static void ShowToast(CryptoPriceAlert alert, decimal currentPrice)
        {
            try
            {
                var direction = alert.Direction == "Above" ? "risen above" : "fallen below";
                new ToastContentBuilder()
                    .AddText($"{alert.Name} Price Alert")
                    .AddText($"{alert.Symbol} has {direction} {alert.TargetPrice.ToString("C", _gbp)}")
                    .AddText($"Current price: {currentPrice.ToString("C", _gbp)}")
                    .Show();
            }
            catch { }
        }

        public static void ResetAlert(string symbol, string direction, decimal targetPrice)
        {
            _notified.Remove($"{symbol}_{direction}_{targetPrice}");
        }
    }
}
