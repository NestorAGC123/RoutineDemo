using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutineDemo.Models
{
    public class CalculateChangeRequest
    {
        public Dictionary<decimal, int> ItemsPricesAndAmounts { get; set; }
        public Dictionary<decimal, int> PaymentProvidedDenominationsAndAmounts { get; set; }
    }

    public class CalculateChangeResponse
    {
        public decimal ChangeDue { get; set; }
        public int OptimumBillsAndCoins { get; set; }

    }
}
