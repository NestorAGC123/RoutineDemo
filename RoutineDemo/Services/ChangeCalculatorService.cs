using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RoutineDemo.Exceptions;
using RoutineDemo.Interfaces;
using RoutineDemo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutineDemo.Services
{
    public class ChangeCalculatorService : IChangeCalculatorService
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<CustomSettingsModel> _customSettings;
        private readonly ILogger _logger;

        public ChangeCalculatorService(IConfiguration configuration, IOptions<CustomSettingsModel> customSettings, ILogger<ChangeCalculatorService> logger)
        {
            _configuration = configuration;
            _customSettings = customSettings;
            _logger = logger;
        }

        /// <summary>
        /// Calculates the change due and the optimum (minimum) number of bills and coins that make it up
        /// </summary>
        /// <remarks>
        /// - This function assumes the minimum denominations are cents
        /// </remarks>
        /// <param name="request">An instance of CalculateChangeRequest with the denominations of bills and coins provided and the price and amount of items</param>
        /// <exception cref="RoutineDemo.Exceptions.SettingNotProvidedException">
        /// Thrown when a required setting is not set.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown when money provided is less than purchased amount or when change cannot be made up with current currency deniminations
        /// </exception>
        /// <returns>An instance of CalculateChangeResponse with the change due and the optimum number of bills and coins that make it up</returns>
        public CalculateChangeResponse CalculateChange(CalculateChangeRequest request)
        {

            decimal totalPurchased = request.ItemsPricesAndAmounts.Sum(x => x.Key * x.Value);
            decimal totalProvided = request.PaymentProvidedDenominationsAndAmounts.Sum(x => x.Key * x.Value);
            if (totalProvided < totalPurchased) throw new InvalidOperationException("Money provided cannot be less than the purchased amount");

            if (_customSettings.Value.Currency == null) throw new SettingNotProvidedException("Currency setting has not been set");
            if (_customSettings.Value.CurrenciesDenominations?.GetValueOrDefault(_customSettings.Value.Currency) is null) throw new SettingNotProvidedException($"Currency denominations setting for currency ({_customSettings.Value.Currency}) has not been set");

            // assuming the lower denominations are cents, convert denominations to integers for better handling
            decimal changeDue = totalProvided - totalPurchased;
            int changeDueAsInt = (int)(changeDue * 100);
            int[] denominationsAsInts = _customSettings.Value.CurrenciesDenominations[_customSettings.Value.Currency].Select(x => (int)( x * 100)).ToArray();

            // looking for the optmimum number of bills and coins, the algorithm is based on a dynamic programming bottom-up approach
            int[] memo = new int[changeDueAsInt + 1].Select(x => int.MaxValue).ToArray();
            memo[0] = 0;
            for (int i = 1; i < changeDueAsInt + 1; i++)
            {
                foreach (int denominationAsInt in denominationsAsInts)
                {
                    if (i - denominationAsInt >= 0 && memo[i - denominationAsInt] != int.MaxValue)
                    {
                        memo[i] = Math.Min(memo[i], 1 + memo[i - denominationAsInt]);
                    }

                }
            }

            // if no optimum number of bills and coins could be found
            if (memo[changeDueAsInt] == int.MaxValue) throw new InvalidOperationException($"Couldn't find an optimum number of bills and coins that add up to the change due ({changeDue}) with current denominations");
            CalculateChangeResponse response = new CalculateChangeResponse() { ChangeDue = changeDue, OptimumBillsAndCoins = memo[changeDueAsInt] };
            return response;
        }

    }
}
