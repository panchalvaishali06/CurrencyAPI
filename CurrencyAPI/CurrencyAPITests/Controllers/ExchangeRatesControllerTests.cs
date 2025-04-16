using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[TestClass]
public class ExchangeRatesControllerTests
{
    private Mock<CurrencyService> _mockService;
    private ExchangeRatesController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockService = new Mock<CurrencyService>();
        _controller = new ExchangeRatesController(_mockService.Object);
    }

    [TestMethod]
    public async Task GetLatestExchangeRatesAsysnc_ReturnsOkResult()
    {
        // Arrange
        var baseCurrency = "USD";
        var mockResult = new { Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } } };
        _mockService.Setup(s => s.GetLatestAsync(baseCurrency)).ReturnsAsync(mockResult);

        // Act
        var result = await _controller.GetLatestExchangeRatesAsysnc(baseCurrency);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(mockResult, okResult.Value);
    }

    [TestMethod]
    public async Task Convert_ReturnsOkResult()
    {
        // Arrange
        var from = "USD";
        var to = "EUR";
        var amount = 100m;
        var mockResult = 85m;
        _mockService.Setup(s => s.ConvertAsync(from, to, amount)).ReturnsAsync(mockResult);

        // Act
        var result = await _controller.Convert(from, to, amount);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual(mockResult, okResult.Value);
    }

    [TestMethod]
    public async Task Convert_ReturnsBadRequest_OnInvalidOperationException()
    {
        // Arrange
        var from = "USD";
        var to = "EUR";
        var amount = 100m;
        var errorMessage = "Invalid conversion.";
        _mockService.Setup(s => s.ConvertAsync(from, to, amount)).ThrowsAsync(new InvalidOperationException(errorMessage));

        // Act
        var result = await _controller.Convert(from, to, amount);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual(errorMessage, ((dynamic)badRequestResult.Value).error);
    }

    [TestMethod]
    public async Task History_ReturnsOkResult()
    {
        // Arrange
        var baseCurrency = "USD";
        var start = new DateTime(2023, 1, 1);
        var end = new DateTime(2023, 1, 10);
        var page = 1;
        var pageSize = 2;
        var mockResult = new List<dynamic>
        {
            new { Rates = new Dictionary<string, decimal> { { "EUR", 0.85m } } },
            new { Rates = new Dictionary<string, decimal> { { "GBP", 0.75m } } },
            new { Rates = new Dictionary<string, decimal> { { "JPY", 110m } } }
        };
        _mockService.Setup(s => s.GetHistoryAsync(baseCurrency, start, end)).ReturnsAsync(mockResult);

        // Act
        var result = await _controller.History(baseCurrency, start, end, page, pageSize);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        var pagedRates = ((IEnumerable<dynamic>)okResult.Value).ToList();
        Assert.AreEqual(2, pagedRates.Count);
    }

    [TestMethod]
    public async Task History_ReturnsBadRequest_OnInvalidDateRange()
    {
        // Arrange
        var baseCurrency = "USD";
        var start = new DateTime(2023, 1, 10);
        var end = new DateTime(2023, 1, 1);

        // Act
        var result = await _controller.History(baseCurrency, start, end);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("Start date must be before end date.", badRequestResult.Value);
    }
}