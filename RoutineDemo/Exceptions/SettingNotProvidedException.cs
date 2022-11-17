using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoutineDemo.Exceptions { 

    /// <summary>
    /// Exception that represent the absensce of a required setting. Settings can be set using the appsettings.json, appsettings.[Environment].json or environment variables
    /// </summary>
    public class SettingNotProvidedException : Exception
    {
        public SettingNotProvidedException(string message): base(message) { }
    }
}
