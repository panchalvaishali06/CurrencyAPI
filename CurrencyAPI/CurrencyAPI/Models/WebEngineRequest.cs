namespace CurrencyAPI.Models
{
	public class WebEngineRequest
	{
		public Uri Url { get; set; }
		public string PostData { get; set; }
		public string ContentType { get; set; }
		public RestType MethodType { get; set; }
		public int TimeOutInMilliSeconds { get; set; }
		public IDictionary<string, string> Headers { get; set; }
		public IDictionary<string, string> FormData { get; set; }
		public HttpContent FormDataContent { get; set; }

		public bool ForceXmlContentType { get; set; }
		public bool ForceJsonContentType { get; set; }
		public bool DisableExpect100Continue { get; set; }

		public string CircuitBreakerName { get; set; }

		public WebEngineRequest()
		{
		}

		public WebEngineRequest
		(
			string cbName,
			RestType type,
			Uri url,
			IDictionary<string, string> headers = null,
			int timeout = 10000,
			string postData = "",
			IDictionary<string, string> formData = null,
			HttpContent formDataContent = null
		)
		{
			Url = url;
			Headers = headers;
			MethodType = type;
			PostData = postData;
			FormData = formData;
			CircuitBreakerName = cbName;
			TimeOutInMilliSeconds = timeout; 
			FormDataContent = formDataContent;
		}
	}
}
