using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceUpdater
{
    public class NotificationEventArgs : EventArgs
    {
        public NotificationEventArgs()
        {
//            UI = new UI();
//            Systray = new Systray();
        }

        public NotificationType NotificationType { get; set; }
        public string Message { get; set; }
//        public UI UI { get; set; }
//        public Systray Systray { get; set; }
        public bool DisableClose { get; set; }
        public bool UserClosed { get; set; }
        public bool ForceRestart { get; set; }
//        public Core.Shared.Enums.DeviceEvent DeviceEvent { get; set; }
//        public Core.Shared.Enums.ACHWorkflow ACHWorkFlow { get; set; }
    }
}
