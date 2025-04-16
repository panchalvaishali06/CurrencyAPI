public interface IExchangeRateProvider
{
    Task<CurrencyResponse> GetLatestRatesAsync(string baseCurrency);
    Task<CurrencyResponse> ConvertAsync(string from, string to, decimal amount);
    Task<List<CurrencyResponse>> GetHistoricalRatesAsync(string baseCurrency, DateTime start, DateTime end);
}
