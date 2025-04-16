using CurrencyAPI.Models;
using CurrencyAPI.Interface;
using CurrencyAPI.Helper;
using LanguageExt;

namespace CurrencyAPI.Service
{
	public sealed class ReselientWebEngine : IReselientWebEngine, IDisposable
	{
		#region Setup

		private readonly List<WebEnginePolicy> _policies;

		public ReselientWebEngine(List<WebEngineProvider> providers)
		{
			_policies = new List<WebEnginePolicy>();

			foreach (var provider in providers)
			{
				_policies.Add(provider.ToPolicy());
			}
		}

        public void Dispose()
        {
            foreach (var policy in _policies)
            {
                policy?.Dispose();
            }
        }
        #endregion

        public async Task<WebEngineResponse> ProcessRequest(WebEngineRequest webReq)
		{
			var policy = _policies.Find(webReq.CircuitBreakerName);

			if (policy?.CircuitBreakerPolicy != null)
			{
				using var cts = new CancellationTokenSource();
				cts.CancelAfter(webReq.GetTimeout());

				var hrm = webReq.ToMessage();
				var res = await policy.CircuitBreakerPolicy.ExecuteAsync
				(
					async () => await policy.Client.SendAsync(hrm, cts.Token)
				);

				return new WebEngineResponse
				(
					await res.Content.ReadAsStringAsync(),
					res.StatusCode,
					res.IsSuccessStatusCode
				);
			}
			else
			{
				return new WebEngineResponse
				(
					$"{"Circuit Breaker not found"}{webReq.CircuitBreakerName}"
				);
			}
		}
	}
}
