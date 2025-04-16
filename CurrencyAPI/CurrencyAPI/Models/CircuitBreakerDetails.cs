namespace CurrencyAPI.Models
{
	public class CircuitBreakerDetails
	{
		public int MinimumThroughput { get; set; }
		public double FailureThreshold { get; set; }
		public TimeSpan DurationOfBreak { get; set; }
		public TimeSpan SamplingDuration { get; set; }

		public CircuitBreakerDetails()
		{
		}

		public CircuitBreakerDetails
		(
			double failureThreshold,
			int minimumThroughput,
			TimeSpan samplingDuration,
			TimeSpan durationOfBreak
		)
		{
			DurationOfBreak = durationOfBreak;
			FailureThreshold = failureThreshold;
			SamplingDuration = samplingDuration;
			MinimumThroughput = minimumThroughput;
		}
	}
}
