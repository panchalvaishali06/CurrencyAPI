using Microsoft.Extensions.Caching.Memory;

public class CurrencyService
{
    private readonly IExchangeRateProvider _provider;
    private readonly IMemoryCache _cache;

    public CurrencyService(IExchangeRateProvider provider, IMemoryCache cache)
    {
        _provider = provider;
        _cache = cache;
    }

    public async Task<object> GetLatestAsync(string baseCurrency)
    {
        var cacheKey = $"LatestRates-{baseCurrency}";
        if (!_cache.TryGetValue(cacheKey, out object rates))
        {
            var response = await _provider.GetLatestRatesAsync(baseCurrency);
            rates = response.Rates;
            _cache.Set(cacheKey, rates, TimeSpan.FromMinutes(10));
        }
        return rates;
    }

    public Task<CurrencyResponse> ConvertAsync(string from, string to, decimal amount)
    {
        return _provider.ConvertAsync(from, to, amount);
    }

    public Task<List<CurrencyResponse>> GetHistoryAsync(string baseCurrency, DateTime start, DateTime end)
    {
        return _provider.GetHistoricalRatesAsync(baseCurrency, start, end);
    }
}
