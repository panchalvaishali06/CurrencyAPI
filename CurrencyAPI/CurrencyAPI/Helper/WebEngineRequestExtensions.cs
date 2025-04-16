using System.Net.Http.Headers;
using LanguageExt;
using CurrencyAPI.Models;

namespace CurrencyAPI.Helper
{
	public static class WebEngineRequestExtensions
	{
		internal static TimeSpan GetTimeout(this WebEngineRequest webReq)
		{
			return TimeSpan.FromMilliseconds
			(
				webReq.TimeOutInMilliSeconds == default ? 10000 : webReq.TimeOutInMilliSeconds
			);
		}

		public static HttpRequestMessage ToMessage(this WebEngineRequest webReq)
		{
			var hrm = new HttpRequestMessage
			(
				webReq.MethodType.ToMethod(),
				webReq.Url
			);

			if (!webReq.PostData.IsNull())
			{
				hrm.Content = new StringContent(webReq.PostData);
			}
			if (webReq.FormData != null)
			{
				hrm.Content = new FormUrlEncodedContent(webReq.FormData);
			}
			if (webReq.DisableExpect100Continue)
			{
				hrm.Headers.Add("Expect", "-");
			}
			if (webReq.FormDataContent != null)
			{
				hrm.Content = webReq.FormDataContent;
			}

			webReq.Headers?.ToList().ForEach(header =>
			{
				switch (header.Key)
				{
					case "Authorization":
						hrm.Headers.Authorization = header.Value.ToAuthHeader();
						break;
					case "RequestHeaderAuthorization":
						hrm.Headers.TryAddWithoutValidation("Authorization", header.Value);
						break;
					case "Content-Type":
						if (hrm.Content != null)
						{
							hrm.Content.Headers.ContentType = new MediaTypeHeaderValue(header.Value.SanitizeContentType());
						}
						break;
					default:
						hrm.Headers.TryAddWithoutValidation(header.Key, header.Value);
						break;
				}
			});

			if (webReq.ForceXmlContentType)
			{
				hrm.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
			}
			else if (webReq.ForceJsonContentType)
			{
				hrm.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			}
			else if (!webReq.ContentType.IsNull() && hrm.Content != null)
			{
				hrm.Content.Headers.ContentType = new MediaTypeHeaderValue(webReq.ContentType.SanitizeContentType());
			}

			if (webReq.FormData != null)
			{
				hrm.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
			}
			return hrm;
		}
        internal static HttpMethod ToMethod(this RestType restType)
        {
            return restType switch
            {
                RestType.GET => HttpMethod.Get,
                RestType.PUT => HttpMethod.Put,
                RestType.POST => HttpMethod.Post,
                RestType.DELETE => HttpMethod.Delete,
                _ => throw new ArgumentOutOfRangeException(nameof(restType)),
            };
        }
        public static string SanitizeContentType(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }
            var split = input.Split(";", StringSplitOptions.RemoveEmptyEntries);
            return split.FirstOrDefault() ?? string.Empty;
        }
        internal static AuthenticationHeaderValue ToAuthHeader(this string input)
        {
            var typeCreds = input.Split(" ");

            if (typeCreds.Length == 2)
            {
                return new AuthenticationHeaderValue(typeCreds[0], typeCreds[1]);
            }
            else
            {
                return new AuthenticationHeaderValue("Basic", input);
            }
        }
    }
}
