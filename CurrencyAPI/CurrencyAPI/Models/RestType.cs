using System.ComponentModel;

namespace CurrencyAPI.Models
{
	public enum RestType
	{
		[Description("GET")]
		GET,

		[Description("POST")]
		POST,

		[Description("DELETE")]
		DELETE,

		[Description("PUT")]
		PUT
	}
}
