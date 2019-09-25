using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeviceUpdater;
using RBA_SDK;
using static DeviceUpdater.Device;

namespace DeviceConfig
{
    class DeviceConfig
    {
        Device Device = new Device();
        DeviceIngenico device = new DeviceIngenico();

        public DeviceConfig(string port)
        {
            // Initialize device
            device.Init(null, null, 0, 0);

            // Reconnect if needed
            if(!device.Connected)
            {
                device.Connect(string.Concat("COM", port));
                //Device.OnNotification += OnNotification;
                Thread.Sleep(100);
            }
        }

        protected void OnNotification(object sender, NotificationEventArgs args)
        {
            switch (args.NotificationType)
            {
                case NotificationType.UI:
                {
                    if(args.UserClosed)
                    {
                        break;
                    }

                    //TODO - need to add configuration check here for unattended - this will suppress the displays

                    break;
                }

                case NotificationType.Systray:
                    break;

                case NotificationType.Log:
                    break;

                case NotificationType.DeviceEvent:
                    break;
                case NotificationType.ACHWorkflow:
                    break;
            }
        }

        public void configRead()
        {
            Console.WriteLine("\r\n24 HOUR REBOOT INFO ----------------------");
            bool enabled = Convert.ToBoolean(Convert.ChangeType(device.Get24RebootState(), typeof(uint)));
            Console.WriteLine($"24 HOUR REBOOT OPTION ENABLED  : {enabled}");
            Console.WriteLine($"DEVICE DAILY REBOOT TIME       : {device.Get24RebootTime()}");
            Console.WriteLine($"DEVICE LAST REBOOT FLAG        : {device.GetLastRebootFlag()}\r\n");
            device.Offline();
            device.Disconnect();
        }

        public void configDevice(string action)
        {
            try
            {
                device.Set24RebootTime(action);
            }
            catch(Exception e)
            {
                Debug.WriteLine("DeviceConfig: EXCEPTION={0}", (object)e.Message);
            }
        }

        public string configVariable(string action)
        {
            string result = string.Empty;
            try
            {
                result = device.GetVariable_29(action);
            }
            catch(Exception e)
            {
                Debug.WriteLine("DeviceConfig: EXCEPTION={0}", (object)e.Message);
            }
            return result;
        }
    }
}
