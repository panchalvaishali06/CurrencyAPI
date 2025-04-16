using CurrencyAPI.Models;
using Polly.CircuitBreaker;

namespace CurrencyAPI.Helper
{
	internal static class WebEnginePolicyExtensions
	{
		internal static WebEnginePolicy Find(this List<WebEnginePolicy> policies, string name)
		{
			return policies.FirstOrDefault(i => i.Name.Equals(name,StringComparison.CurrentCultureIgnoreCase));
		}

		internal static AsyncCircuitBreakerPolicy FindCBP(this List<WebEnginePolicy> policies, string name)
		{
			return policies.Find(name)?.CircuitBreakerPolicy;
		}
	}
}
