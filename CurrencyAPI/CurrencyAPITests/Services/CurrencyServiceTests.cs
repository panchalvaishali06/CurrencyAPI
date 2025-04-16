using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[TestClass]
public class CurrencyServiceTests
{
    private Mock<IExchangeRateProvider> _mockProvider;
    private IMemoryCache _memoryCache;
    private CurrencyService _service;

    [TestInitialize]
    public void Setup()
    {
        _mockProvider = new Mock<IExchangeRateProvider>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _service = new CurrencyService(_mockProvider.Object, _memoryCache);
    }

    [TestMethod]
    public async Task GetLatestAsync_ReturnsRatesFromCache()
    {
        // Arrange
        var baseCurrency = "USD";
        var cacheKey = $"LatestRates-{baseCurrency}";
        var cachedRates = new { Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } } };
        _memoryCache.Set(cacheKey, cachedRates);

        // Act
        var result = await _service.GetLatestAsync(baseCurrency);

        // Assert
        Assert.AreEqual(cachedRates, result);
        _mockProvider.Verify(p => p.GetLatestRatesAsync(It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public async Task GetLatestAsync_FetchesRatesWhenNotInCache()
    {
        // Arrange
        var baseCurrency = "USD";
        var cacheKey = $"LatestRates-{baseCurrency}";
        var providerResponse = new { Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } } };
        _mockProvider.Setup(p => p.GetLatestRatesAsync(baseCurrency)).ReturnsAsync(providerResponse);

        // Act
        var result = await _service.GetLatestAsync(baseCurrency);

        // Assert
        Assert.AreEqual(providerResponse.Rates, result);
        Assert.IsTrue(_memoryCache.TryGetValue(cacheKey, out _));
        _mockProvider.Verify(p => p.GetLatestRatesAsync(baseCurrency), Times.Once);
    }

    [TestMethod]
    public async Task ConvertAsync_CallsProvider()
    {
        // Arrange
        var from = "USD";
        var to = "EUR";
        var amount = 100m;
        var providerResponse = new CurrencyResponse { ConvertedAmount = 85m };
        _mockProvider.Setup(p => p.ConvertAsync(from, to, amount)).ReturnsAsync(providerResponse);

        // Act
        var result = await _service.ConvertAsync(from, to, amount);

        // Assert
        Assert.AreEqual(providerResponse, result);
        _mockProvider.Verify(p => p.ConvertAsync(from, to, amount), Times.Once);
    }

    [TestMethod]
    public async Task GetHistoryAsync_CallsProvider()
    {
        // Arrange
        var baseCurrency = "USD";
        var start = new DateTime(2023, 1, 1);
        var end = new DateTime(2023, 1, 10);
        var providerResponse = new List<CurrencyResponse>
        {
            new CurrencyResponse { Date = start, Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } } },
            new CurrencyResponse { Date = end, Rates = new Dictionary<string, decimal> { { "GBP", 0.75m } } }
        };
        _mockProvider.Setup(p => p.GetHistoricalRatesAsync(baseCurrency, start, end)).ReturnsAsync(providerResponse);

        // Act
        var result = await _service.GetHistoryAsync(baseCurrency, start, end);

        // Assert
        Assert.AreEqual(providerResponse, result);
        _mockProvider.Verify(p => p.GetHistoricalRatesAsync(baseCurrency, start, end), Times.Once);
    }
}
