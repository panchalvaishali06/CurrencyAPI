using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ExchangeRatesController : ControllerBase
{
    private readonly CurrencyService _service;

    public ExchangeRatesController(CurrencyService service)
    {
        _service = service;
    }

    [HttpGet("latestExRate/v1")]
    [Authorize]
    public async Task<IActionResult> GetLatestExchangeRatesAsysnc([FromQuery] string baseCurrency)
    {
        var result = await _service.GetLatestAsync(baseCurrency);
        return Ok(result);
    }

    [HttpGet("convert/v1")]
    [Authorize]
    public async Task<IActionResult> Convert([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount)
    {
        try
        {
            var result = await _service.ConvertAsync(from, to, amount);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("history/v1")]
    [Authorize]
    public async Task<IActionResult> History([FromQuery] string baseCurrency, [FromQuery] DateTime start, [FromQuery] DateTime end,[FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (start > end)
        {
            return BadRequest("Start date must be before end date.");
        }
        var result = await _service.GetHistoryAsync(baseCurrency, start, end);
        var pagedRates = result.Select(x=>x.Rates).Skip((page - 1) * pageSize).Take(pageSize);
        return Ok(pagedRates);
    }
}
