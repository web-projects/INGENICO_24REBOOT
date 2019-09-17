using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceUpdater
{
    public enum IngenicoLoggingLevel
    {
        NONE = -1,
        ERROR = 0,
        WARNING = 1,
        INFO = 2,
        TRACE = 3,
        DEBUG = 4
    };

    public enum NotificationType
    {
        UI,
        Systray,
        Log,
        DeviceEvent,
        ACHWorkflow
    }
}
