using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RoutineDemo.Exceptions;
using RoutineDemo.Models;
using RoutineDemo.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace Tests.Services
{
    public class ChangeCalculatorTests
    {
        private readonly ChangeCalculatorService _changeCalculatorService;
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<IOptions<CustomSettingsModel>> _customSettingsMock = new Mock<IOptions<CustomSettingsModel>>();
        private readonly Mock<ILogger<ChangeCalculatorService>> _loggerMock = new Mock<ILogger<ChangeCalculatorService>>();

        public ChangeCalculatorTests()
        {
            _changeCalculatorService = new ChangeCalculatorService(_configurationMock.Object, _customSettingsMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void CalculateChange_ShouldThrowInvalidOperationException_WhenMoneyProvidedIsLessThanPurchasePrice()
        {

            Dictionary<decimal, int> itemsPriceAndAmount = new Dictionary<decimal, int> {
                { 0.5m, 1},
                { 100m, 2}
            };
            Dictionary<decimal, int> moneyProvided = new Dictionary<decimal, int>
            {
                { 1m, 1},
                { 10m, 2}
            };
            var calculateChangeRequest = new CalculateChangeRequest() { ItemsPricesAndAmounts = itemsPriceAndAmount, PaymentProvidedDenominationsAndAmounts = moneyProvided };

            Assert.Throws<InvalidOperationException>(() => _changeCalculatorService.CalculateChange(calculateChangeRequest));
        }

        [Fact]
        public void CalculateChange_ShouldThrowSettingNotProvidedException_WhenCurrencySettingIsNotSet()
        {
           
            _customSettingsMock.Setup(x => x.Value).Returns(new CustomSettingsModel());

            Dictionary<decimal, int> itemsPriceAndAmount = new Dictionary<decimal, int> {
                { 0.5m, 1},
                { 10m, 2}
            };
            Dictionary<decimal, int> moneyProvided = new Dictionary<decimal, int>
            {
                { 1m, 1},
                { 10m, 2}
            };
            var calculateChangeRequest = new CalculateChangeRequest() { ItemsPricesAndAmounts = itemsPriceAndAmount, PaymentProvidedDenominationsAndAmounts = moneyProvided };

            Assert.Throws<SettingNotProvidedException>(() => _changeCalculatorService.CalculateChange(calculateChangeRequest));
        }

        [Fact]
        public void CalculateChange_ShouldThrowSettingNotProvidedException_WhenDenominationsSettingIsNotSet()
        {

            _customSettingsMock.Setup(x => x.Value).Returns(new CustomSettingsModel() { Currency = "USD"});

            Dictionary<decimal, int> itemsPriceAndAmount = new Dictionary<decimal, int> {
                { 0.5m, 1},
                { 10m, 2}
            };
            Dictionary<decimal, int> moneyProvided = new Dictionary<decimal, int>
            {
                { 1m, 1},
                { 10m, 2}
            };
            var calculateChangeRequest = new CalculateChangeRequest() { ItemsPricesAndAmounts = itemsPriceAndAmount, PaymentProvidedDenominationsAndAmounts = moneyProvided };

            Assert.Throws<SettingNotProvidedException>(() => _changeCalculatorService.CalculateChange(calculateChangeRequest));
        }

        [Fact]
        public void CalculateChange_ShouldThrowSettingNotProvidedException_WhenDenominationsForCurrencySettingIsNotSet()
        {

            _customSettingsMock.Setup(x => x.Value).Returns(new CustomSettingsModel() { 
                Currency = "USD", 
                CurrenciesDenominations = new Dictionary<string, decimal[]> { { "MXN", new decimal[0] } } 
            });

            Dictionary<decimal, int> itemsPriceAndAmount = new Dictionary<decimal, int> {
                { 0.5m, 1},
                { 10m, 2}
            };
            Dictionary<decimal, int> moneyProvided = new Dictionary<decimal, int>
            {
                { 1m, 1},
                { 10m, 2}
            };
            var calculateChangeRequest = new CalculateChangeRequest() { ItemsPricesAndAmounts = itemsPriceAndAmount, PaymentProvidedDenominationsAndAmounts = moneyProvided };

            Assert.Throws<SettingNotProvidedException>(() => _changeCalculatorService.CalculateChange(calculateChangeRequest));
        }

        [Fact]
        public void CalculateChange_ShouldThrowInvalidOperationException_WhenNoOptimumBillsAndCoins()
        {

            _customSettingsMock.Setup(x => x.Value).Returns(new CustomSettingsModel()
            {
                Currency = "USD",
                CurrenciesDenominations = new Dictionary<string, decimal[]> { { "USD", new decimal[3] { 1m, 2m, 3m } } }
            });

            Dictionary<decimal, int> itemsPriceAndAmount = new Dictionary<decimal, int> {
                { 10m, 1},
                { 0.5m, 3}
            };
            Dictionary<decimal, int> moneyProvided = new Dictionary<decimal, int>
            {
                { 20m, 1},
                { 10m, 2}
            };
            var calculateChangeRequest = new CalculateChangeRequest() { ItemsPricesAndAmounts = itemsPriceAndAmount, PaymentProvidedDenominationsAndAmounts = moneyProvided };

            Assert.Throws<InvalidOperationException>(() => _changeCalculatorService.CalculateChange(calculateChangeRequest));
        }

        [Theory]
        [InlineData("28.5", 11 ) ]
        public void CalculateChange_ShouldReturnChangeDueAndOptimumBillsAndCoins(string ChangeDue, int NumBillsAndCoins)
        {
            _customSettingsMock.Setup(x => x.Value).Returns(new CustomSettingsModel()
            {
                Currency = "USD",
                CurrenciesDenominations = new Dictionary<string, decimal[]> { { "USD", new decimal[4] { 0.5m, 1m, 2m, 3m } } }
            });

            Dictionary<decimal, int> itemsPriceAndAmount = new Dictionary<decimal, int> {
                { 10m, 1},
                { 0.5m, 3}
            };
            Dictionary<decimal, int> moneyProvided = new Dictionary<decimal, int>
            {
                { 20m, 1},
                { 10m, 2}
            };
            var calculateChangeRequest = new CalculateChangeRequest() { ItemsPricesAndAmounts = itemsPriceAndAmount, PaymentProvidedDenominationsAndAmounts = moneyProvided };
            var calculateChangeResponse = _changeCalculatorService.CalculateChange(calculateChangeRequest);

            Assert.Equal(Decimal.Parse(ChangeDue), calculateChangeResponse.ChangeDue);
            Assert.Equal(NumBillsAndCoins, calculateChangeResponse.OptimumBillsAndCoins);
        }
    }
}
