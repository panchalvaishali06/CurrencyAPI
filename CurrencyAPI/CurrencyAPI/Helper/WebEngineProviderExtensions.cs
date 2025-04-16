using Polly;
using CurrencyAPI.Models;

namespace CurrencyAPI.Helper
{
	internal static class WebEngineProviderExtensions
	{
		internal static WebEnginePolicy ToPolicy(this WebEngineProvider provider)
		{
			var client = new HttpClient();

			var circuitBreaker = Policy.Handle<Exception>().AdvancedCircuitBreakerAsync
			(
				provider.CircuitBreakerDetails.FailureThreshold,
				provider.CircuitBreakerDetails.SamplingDuration,
				provider.CircuitBreakerDetails.MinimumThroughput,
				provider.CircuitBreakerDetails.DurationOfBreak
			);
			return new WebEnginePolicy(provider.Name, client, circuitBreaker);
		}
    }
}
