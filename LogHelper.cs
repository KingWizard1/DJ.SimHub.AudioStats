using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DJ.SimHub
{
    public static class LogHelper
    {

        public static void Log(string message)
        {
            global::SimHub.Logging.Current.Info(message);
        }

        public static void Error(string message)
        {
            global::SimHub.Logging.Current.Error(message);
        }

    }
}
