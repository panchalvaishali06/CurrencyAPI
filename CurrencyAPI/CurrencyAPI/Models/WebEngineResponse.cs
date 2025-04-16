using System.Net;

namespace CurrencyAPI.Models
{
	public class WebEngineResponse
	{
		public string Response { get; set; }
		public byte[] ResponseBytes { get; set; }
		public bool IsSuccessStatus { get; set; }
		public HttpStatusCode StatusCode { get; set; }

		public WebEngineResponse()
		{
		}

		public WebEngineResponse(string response)
		{
			Response = response;
			IsSuccessStatus = false;
			StatusCode = HttpStatusCode.PreconditionFailed;
		}

		public WebEngineResponse(string response, HttpStatusCode statusCode, bool isSuccess)
		{
			Response = response;
			StatusCode = statusCode;
			IsSuccessStatus = isSuccess;
		}
	}
}
