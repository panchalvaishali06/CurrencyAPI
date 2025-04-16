using CurrencyAPI.Models;
namespace CurrencyAPI.Interface
{
	public interface IReselientWebEngine
	{
		Task<WebEngineResponse> ProcessRequest(WebEngineRequest webReq);
	}
}
