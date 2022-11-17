using RoutineDemo.Models;

namespace RoutineDemo.Interfaces
{
    public interface IChangeCalculatorService
    {
        CalculateChangeResponse CalculateChange(CalculateChangeRequest request);
    }
}