using CurrencyAPI.Models;
using CurrencyAPI.Service;
using CurrencyAPI.Interface;

namespace CurrencyAPI.Helper
{
    public static class ConfigHelper
    {
        public static Dictionary<string, CircuitBreakerDetails> GetCircuitBreakerDetails(this IConfiguration config)
        {
            string CurrencyCircuitBreaker = "CurrencyCircuitBreaker";
            string CurrencyCBSection = "Currency:CircuitBreaker";
            
        var settings = new Dictionary<string, CircuitBreakerDetails>();
            
            if (config.GetSection(CurrencyCBSection).Exists())
            {
                settings.Add(CurrencyCircuitBreaker, config.GetCircuitBreakerDetail(CurrencyCBSection));
            }

            return settings;
        }

        public static CircuitBreakerDetails GetCircuitBreakerDetail(this IConfiguration config, string section)
        {
        
            int.TryParse(config[$"{section}:MinimumThroughput"], out int minimumThroughput);
            double.TryParse(config[$"{section}:FailureThreshold"], out double failureThreshold);
            int.TryParse(config[$"{section}:DurationOfBreakSeconds"], out int durationOfBreakSeconds);
            int.TryParse(config[$"{section}:SamplingDuration"], out int samplingDurationSeconds);

            var details = new CircuitBreakerDetails
            {
                FailureThreshold = failureThreshold,
                MinimumThroughput = minimumThroughput,
                SamplingDuration = TimeSpan.FromSeconds(samplingDurationSeconds),
                DurationOfBreak = TimeSpan.FromSeconds(durationOfBreakSeconds)
            };

            return details;
        }

        public static void AddResilientWebEngine(this IServiceCollection collection, Dictionary<string, CircuitBreakerDetails> details)
        {
            collection.AddSingleton<IReselientWebEngine>(new ReselientWebEngine(details.ToProviders()));
        }
        internal static List<WebEngineProvider> ToProviders(this Dictionary<string, CircuitBreakerDetails> input)
        {
            return input?.Select(p => new WebEngineProvider(p.Key, p.Value)).ToList();
        }
    }
}