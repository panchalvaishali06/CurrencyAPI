using Polly.CircuitBreaker;

namespace CurrencyAPI.Models
{
	public sealed class WebEnginePolicy : IDisposable
	{
		public string Name { get; set; }
		public HttpClient Client { get; set; }
		public AsyncCircuitBreakerPolicy CircuitBreakerPolicy { get; set; }

		public WebEnginePolicy()
		{
		}

		public WebEnginePolicy(string name, HttpClient client, AsyncCircuitBreakerPolicy cbp)
		{
			Name = name;
			Client = client;
			CircuitBreakerPolicy = cbp;
		}

		public void Dispose()
		{
			Client?.Dispose();
		}
	}
}
