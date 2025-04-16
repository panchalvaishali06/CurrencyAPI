namespace CurrencyAPI.Models
{
	public class WebEngineProvider
	{
		public string Name { get; set; }
		public CircuitBreakerDetails CircuitBreakerDetails { get; set; }

		public WebEngineProvider()
		{
		}

		public WebEngineProvider(string name, CircuitBreakerDetails details)
		{
			Name = name;
			CircuitBreakerDetails = details;
		}
	}
}
