# Routine Demo

All projects in this repository depend on .Net 6. <br>
To see the document this project is based on see: [Cash register coding assignment](cash_register_coding_assingment.txt).


## Routine Demo project
This project is the entry point of the application (a console application) and contains bussiness related code, like the following:
- Models: DTOs and models
- Exceptions: custom exceptions
- Interfaces
- Services: all services should implement a corresponding interface in order to take advantage of dependency injection and mocking patterns.
- appsettings.json: global configurations are set here


### Important configurations in appsettings.json
Configurations for the logger service should be set in the Logging section. To learn more about logging in .Net and available configs see [Logging in .Net](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line)
```
  {
    "Logging": {
        "LogLevel": {
        "Default": "Information",
        "RoutineDemo.Services": "Warning"
        }
    }
    ... Other configs ...
  }
```


Configurations for the application should be set in the CustomSettings section.

- CurrenciesDenominations: key/value mappings where the key is the currency identifier and the value is an array with the supported denominations of bills and coins.
- Currency: currency identifier for the application to use. It should be present in the CurrenciesDenominations setting. 

```
  {
    ... Other configs ...

    "CustomSettings": {
        "Currency": "USD",
        "CurrenciesDenominations": {
            "USD": [ 0.01, 0.05, 0.10, 0.25, 0.50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00 ],
            "MXN": [ 0.05, 0.10, 0.20, 0.50, 1.00, 2.00, 5.00, 10.00, 20.00, 50.00, 100.00 ]
        }
    }
  }
```
## Tests project
Test project is based on XUnit as testing framework and Moq for mocking dependencies. Current coverage is as follows:
- RoutineDemo
    - Services
        - ChangeCalculator

