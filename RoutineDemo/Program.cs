using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RoutineDemo.Interfaces;
using RoutineDemo.Models;
using RoutineDemo.Services;

IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices((context, services) => {
        services.AddSingleton<IChangeCalculatorService,ChangeCalculatorService>();
        services.Configure<CustomSettingsModel>(context.Configuration.GetSection("CustomSettings"));
    })
    .Build();


var service = ActivatorUtilities.CreateInstance<ChangeCalculatorService>(host.Services);
var logger = host.Services.GetRequiredService<ILogger<Program>>();


Console.WriteLine("Insert items prices and amounts in the format: 'price,amount price,amount ...'");
Dictionary<decimal, int> itemsPriceAndAmount = new Dictionary<decimal, int>();

bool input1Correct = false;
while(!input1Correct)
{
    try
    {
        string[][] priceAmountPairs = Console.ReadLine().TrimEnd().Split(' ').Select(x => x.Split(',')).ToArray();
        foreach (string[] priceAmountPair in priceAmountPairs)
        {
            itemsPriceAndAmount.Add(decimal.Parse(priceAmountPair[0]), int.Parse(priceAmountPair[1]));
        }

        input1Correct = true;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Input was malformed, try again");
    }

}



Console.WriteLine("Insert money provided as list of denominations and amounts in the format: 'denomination,amount denomination,amount ...'");
Dictionary<decimal, int> moneyProvided = new Dictionary<decimal, int>();

bool input2Correct = false;
while (!input2Correct)
{
    try
    {
        string[][] denominationAmountPairs = Console.ReadLine().TrimEnd().Split(' ').Select(x => x.Split(',')).ToArray();
        foreach (string[] denominationAmountPair in denominationAmountPairs)
        {
            moneyProvided.Add(decimal.Parse(denominationAmountPair[0]), int.Parse(denominationAmountPair[1]));
        }

        input2Correct = true;
    }
    catch (Exception ex)
    {
        Console.WriteLine("Input was malformed, try again");
    }

}



var calculateChangeRequest = new CalculateChangeRequest() { ItemsPricesAndAmounts = itemsPriceAndAmount, PaymentProvidedDenominationsAndAmounts = moneyProvided };

try
{
    var calculateChangeResponse = service.CalculateChange(calculateChangeRequest);
    logger.LogInformation("Change due is: {}", calculateChangeResponse.ChangeDue);
    logger.LogInformation("Optimum number of bills and coins to return: {}", calculateChangeResponse.OptimumBillsAndCoins);
}
catch (Exception ex)
{
    logger.LogError(ex.Message);
}





