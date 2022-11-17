using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutineDemo.Models
{
    public class CustomSettingsModel
    {
        public string Currency { get; set; }
        public Dictionary<string, decimal[]> CurrenciesDenominations { get; set; }
    }
}
