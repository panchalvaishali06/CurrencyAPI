using Newtonsoft.Json;
using CurrencyAPI.Models;
using CurrencyAPI.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

public class FrankfurterProvider : IExchangeRateProvider
{
    private readonly IReselientWebEngine _webEngine;
    private readonly IConfiguration _config;
    private static readonly JsonSerializerSettings _settings = new()
    {
        NullValueHandling = NullValueHandling.Ignore
    };
    private readonly ILogger<FrankfurterProvider> _logger;
    string _managerName;
    private const string CurrencyCB = "CurrencyCircuitBreaker";
    private const string ContentType = "application/json";
    public FrankfurterProvider(IReselientWebEngine webEngine, ILogger<FrankfurterProvider> logger, IConfiguration config)
    {
        _webEngine = webEngine;
        _logger = logger;
        _config = config;
        _managerName = nameof(FrankfurterProvider);
    }

    public async Task<CurrencyResponse> GetLatestRatesAsync(string baseCurrency)
    {
        var methodName = $"{_managerName}_{nameof(GetLatestRatesAsync)}";
        try
        {
            var request = new WebEngineRequest
            {
                Url = new Uri($"{_config["Currency:BaseUrl"]}latest?base={baseCurrency}"),
                MethodType = RestType.GET,
                ContentType = ContentType,
                CircuitBreakerName = CurrencyCB,
                TimeOutInMilliSeconds = 35000
            };
            var response = await _webEngine.ProcessRequest(request);
            if (response.IsSuccessStatus)
            {
                return JsonConvert.DeserializeObject<CurrencyResponse>(response.Response, _settings);
            }
            else
            {
                _logger.LogError($"{methodName}_{nameof(response.StatusCode)}", response.Response);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{methodName}", ex.StackTrace);
            return default;
        }
    }

    public async Task<CurrencyResponse> ConvertAsync(string from, string to, decimal amount)
    {
        var methodName = $"{_managerName}_{nameof(ConvertAsync)}";

        try
        {
            List<string> excludedCurrencies = new List<string> { "TRY", "PLN", "THB", "MXN" };
            if (excludedCurrencies.Contains(from) || excludedCurrencies.Contains(to))
            {
                throw new InvalidOperationException("Unsupported currency.");
            }
            var request = new WebEngineRequest
            {
                Url = new Uri($"{_config["Currency:BaseUrl"]}latest?from={from}&to={to}"),
                MethodType = RestType.GET,
                ContentType = ContentType,
                CircuitBreakerName = CurrencyCB,
                TimeOutInMilliSeconds = 35000
            };
            var response = await _webEngine.ProcessRequest(request);
            if (response.IsSuccessStatus)
            {
                var serializedRes = JsonConvert.DeserializeObject<CurrencyResponse>(response.Response, _settings);
                if (serializedRes!.Rates == null || !serializedRes!.Rates.ContainsKey(to))
                {
                    _logger.LogError($"{methodName}_{nameof(serializedRes)}_Bad Request", serializedRes);
                }

                serializedRes!.Rates.TryGetValue(to, out var rate);
                
                return new CurrencyResponse
                {
                    Base = from,
                    Date = serializedRes.Date,
                    Rates = new Dictionary<string, decimal> { { to, rate * amount } }
                };
            }
            else
            {
                _logger.LogError($"{methodName}_{nameof(response.StatusCode)}", response.Response);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{methodName}", ex.StackTrace);
            return default;
        }
    }

    public async Task<List<CurrencyResponse>> GetHistoricalRatesAsync(string baseCurrency, DateTime start, DateTime end)
    {
        var methodName = $"{_managerName}_{nameof(GetHistoricalRatesAsync)}";
        try
        {
            var request = new WebEngineRequest
            {
                Url = new Uri($"{_config["Currency:BaseUrl"]}{start:yyyy-MM-dd}..{end:yyyy-MM-dd}?base={baseCurrency}"),
                MethodType = RestType.GET,
                ContentType = ContentType,
                CircuitBreakerName = CurrencyCB,
                TimeOutInMilliSeconds = 35000
            };
            var response = await _webEngine.ProcessRequest(request);
            if (response.IsSuccessStatus)
            {
                return JsonConvert.DeserializeObject<List<CurrencyResponse>>(response.Response, _settings);
            }
            else
            {
                _logger.LogError($"{methodName}_{nameof(response.StatusCode)}", response.Response);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"{methodName}", ex.StackTrace);
            return default;
        }
    }
}
