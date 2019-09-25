using RBA_SDK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static DeviceUpdater.Device;

namespace DeviceUpdater
{
    class DeviceIngenico
    {
        private Device ingenicoDevice;
        private Device.Health device_health;
        private Device.Info device_info;

        private string filePath;
        private string result;
        public  bool Connected = false;

        public string Result { get; set; }

        public event EventHandler<NotificationEventArgs> OnNotification = delegate {};
        public delegate void EventHandler(object sender, NotificationEventArgs args);

        public void Init(string[] accepted, string[] available, int baudRate, int dataBits)
        {
//            acceptedPorts = accepted;
//            availablePorts = available;

            ingenicoDevice = new Device();
            device_health = new Device.Health();
            device_info = new Device.Info();

            //Add event handlers
            ingenicoDevice.ComBaudRate = baudRate;
            ingenicoDevice.ComDataBits = dataBits;
//            ingenicoDevice.DeviceInputReceived += (sender3, deviceArgs) => DeviceInputReceived(deviceArgs.MessageID, deviceArgs.DeviceForm, deviceArgs.KeyPressID);
//            ingenicoDevice.DeviceConnectionChanged += (sender4, deviceConnectionArgs) => UpdateDeviceIngenico(deviceConnectionArgs.ConnectionStatus);

            string fullName = Assembly.GetEntryAssembly().Location;
            string logname = System.IO.Path.GetFileNameWithoutExtension(fullName) + ".log";
            string path = System.IO.Directory.GetCurrentDirectory(); 
            filePath = path + "\\" + logname;
        }

        public bool Connect(string port)
        {
            result = ingenicoDevice.Connect(port, Logger, IngenicoLoggingLevel.ERROR);
            Thread.Sleep(2000);

            Debug.WriteLine("device connect: {0} --------------------------------------------------", (object) result);

            Connected = ingenicoDevice.Connected;

            if(Connected)
            {
                device_health.GetDeviceHealth();
                device_info.GetDeviceInfo();

                // DUMP DEVICE INFO
                Debug.WriteLine("MANUFACTURE                    : {0}", (object) device_info.MANUFACTURE);
                Debug.WriteLine("DEVICE                         : {0}", (object) device_info.DEVICE);
                Debug.WriteLine("UNIT_SERIAL_NUMBER             : {0}", (object) device_info.UNIT_SERIAL_NUMBER);
                Debug.WriteLine("RAM_SIZE                       : {0}", (object) device_info.RAM_SIZE);
                Debug.WriteLine("FLASH_SIZE                     : {0}", (object) device_info.FLASH_SIZE);
                Debug.WriteLine("DIGITIZER_VERSION              : {0}", (object) device_info.DIGITIZER_VERSION);
                Debug.WriteLine("SECURITY_MODULE_VERSION        : {0}", (object) device_info.SECURITY_MODULE_VERSION);
                Debug.WriteLine("OS_VERSION                     : {0}", (object) device_info.OS_VERSION);
                Debug.WriteLine("APPLICATION_VERSION            : {0}", (object) device_info.APPLICATION_VERSION);
                Debug.WriteLine("EFTL_VERSION                   : {0}", (object) device_info.EFTL_VERSION);
                Debug.WriteLine("EFTP_VERSION                   : {0}", (object) device_info.EFTP_VERSION);
                Debug.WriteLine("MANUFACTURING_SERIAL_NUMBER    : {0}", (object) device_info.MANUFACTURING_SERIAL_NUMBER);
                Debug.WriteLine("EMV_DC_KERNEL_TYPE             : {0}", (object) device_info.EMV_DC_KERNEL_TYPE);
                Debug.WriteLine("EMV_ENGINE_KERNEL_TYPE         : {0}", (object) device_info.EMV_ENGINE_KERNEL_TYPE);
                Debug.WriteLine("CLESS_DISCOVER_KERNEL_TYPE     : {0}", (object) device_info.CLESS_DISCOVER_KERNEL_TYPE);
                Debug.WriteLine("CLESS_EXPRESSPAY_V2_KERNEL_TYPE: {0}", (object) device_info.CLESS_EXPRESSPAY_V2_KERNEL_TYPE);
                Debug.WriteLine("CLESS_EXPRESSPAY_V3_KERNEL_TYPE: {0}", (object) device_info.CLESS_EXPRESSPAY_V3_KERNEL_TYPE);
                Debug.WriteLine("CLESS_PAYPASS_V3_KERNEL_TYPE   : {0}", (object) device_info.CLESS_PAYPASS_V3_KERNEL_TYPE);
                Debug.WriteLine("CLESS_PAYPASS_V3_APP_TYPE      : {0}", (object) device_info.CLESS_PAYPASS_V3_APP_TYPE);
                Debug.WriteLine("CLESS_VISA_PAYWAVE_KERNEL_TYPE : {0}", (object) device_info.CLESS_VISA_PAYWAVE_KERNEL_TYPE);
                Debug.WriteLine("CLESS_INTERAC_KERNEL_TYPE      : {0}", (object) device_info.CLESS_INTERAC_KERNEL_TYPE);
            }

            return (result.Equals("RESULT_SUCCESS")) ? true : false;
        }

        public void Disconnect()
        {
            try
            {
                if (RBA_API.GetConnectionStatus().Equals(CONNECTION_STATUS.CONNECTED))
                {
                    Result = RBA_API.Disconnect().ToString();
                    ingenicoDevice.DeviceConnectionEvent();
                    Connected = ingenicoDevice.Connected;
                    Debug.WriteLine("device disconnect: connected={0} --------------------------------------------------", Connected);
                }
            }
            catch (Exception)
            {
                //log error
                //if (ex.Source == "RBA_SDK_CS")
                    //MessageBox.Show("Exception Occured during Disconnect:" + Environment.NewLine + ex.ToString() + Environment.NewLine + Environment.NewLine + "Suggested Resolution:" + Environment.NewLine + "For 32-bit machine: Add directory path for the dll to Environment variable PATH" + Environment.NewLine + "For 64-bit machine: Copy the dll to C:\\Windows\\SysWOW64");
            }

        }

        private void Logger(string message)
        {
            Console.WriteLine("{0}", message);

            using (StreamWriter streamWriter = new StreamWriter(filePath, append: true))
            {
                streamWriter.WriteLine(message);
                streamWriter.Close();
            }

            //TODO: determin how this is used (passed to Ingenico?)
            NotificationRaise(new NotificationEventArgs
            {
                NotificationType = NotificationType.Log,
                Message = message
            });
        }

        public void NotificationRaise(NotificationEventArgs e)
        {
            OnNotification?.Invoke(null, e);
        }

        public ERROR_ID LoadForm(string formName, string message)
        {
            ERROR_ID Result = RBA_API.ProcessMessage(MESSAGE_ID.M01_ONLINE);;

            Debug.WriteLine("device online : {0} --------------------------------------------------", (object) result);

            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_FORM_NUMBER, formName);

            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_TYPE_OF_ELEMENT, StringEnum.GetStringValue(DeviceFormTypeOf.Text));
            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_TEXT_ELEMENTID,  StringEnum.GetStringValue(DeviceFormElementID.PromptLine1));
            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_PROMPT_IDX, message);

            Result = RBA_API.ProcessMessage(MESSAGE_ID.M24_FORM_ENTRY);

            //Make sure the form shows on the device long enough for the user to see it
            //System.Threading.Thread.Sleep(4000);

            return Result;
        }

        public ERROR_ID ShowMessage(string message)
        {
            ERROR_ID Result;

            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_FORM_NUMBER, StringEnum.GetStringValue(DeviceForms.Message));

            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_TYPE_OF_ELEMENT, StringEnum.GetStringValue(DeviceFormTypeOf.Text));
            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_TEXT_ELEMENTID, StringEnum.GetStringValue(DeviceFormElementID.PromptLine1));
            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_PROMPT_IDX, message);

            Result = RBA_API.ProcessMessage(MESSAGE_ID.M24_FORM_ENTRY);

            //Make sure the form shows on the device long enough for the user to see it
            Thread.Sleep(2000);

            return Result;
        }

        public ERROR_ID ShowMessage(DeviceForms form, string message)
        {
            ERROR_ID Result;

            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_FORM_NUMBER, StringEnum.GetStringValue(form));

            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_TYPE_OF_ELEMENT, StringEnum.GetStringValue(DeviceFormTypeOf.Text));
            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_TEXT_ELEMENTID, StringEnum.GetStringValue(DeviceFormElementID.PromptLine1));
            Result = RBA_API.SetParam(PARAMETER_ID.P24_REQ_PROMPT_IDX, message);

            Result = RBA_API.ProcessMessage(MESSAGE_ID.M24_FORM_ENTRY);

            //Make sure the form shows on the device long enough for the user to see it
            Thread.Sleep(2000);

            return Result;
        }

        public ERROR_ID HardDeviceReset()
        {
            ERROR_ID Result = RBA_API.ProcessMessage(MESSAGE_ID.M97_REBOOT);
            return Result;
        }

        public void Reset()
        {
            ERROR_ID Result = RBA_API.ProcessMessage(MESSAGE_ID.M10_HARD_RESET);
        }

        public ERROR_ID UpdateDevice(string formFilePath)
        {
            string filename = Path.GetFileName(formFilePath);
            RBA_API.SetParam(PARAMETER_ID.P62_REQ_RECORD_TYPE, "0");
            RBA_API.SetParam(PARAMETER_ID.P62_REQ_ENCODING_FORMAT, "8");

            //this will be a tgz file for all forms - application files are set to 1
            if (formFilePath.ToLower().Contains("ogz"))
            {
                RBA_API.SetParam(PARAMETER_ID.P62_REQ_UNPACK_FLAG, "0");
            }
            else
            {
                RBA_API.SetParam(PARAMETER_ID.P62_REQ_UNPACK_FLAG, "1");
            }

            RBA_API.SetParam(PARAMETER_ID.P62_REQ_FAST_DOWNLOAD, "1");
            RBA_API.SetParam(PARAMETER_ID.P62_REQ_OS_FILE_NAME, formFilePath);
            RBA_API.SetParam(PARAMETER_ID.P62_REQ_FILE_NAME, filename);

            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.FILE_WRITE);

            return result;
        }

        public ERROR_ID UpdateDeviceCustom(string form_file_path)
        {
            ERROR_ID Result = ERROR_ID.RESULT_SUCCESS;

            try
            {
                string filename = Path.GetFileName(form_file_path);
                Result = RBA_API.SetParam(PARAMETER_ID.P62_REQ_UNPACK_FLAG, "0");
                Result = RBA_API.SetParam(PARAMETER_ID.P62_REQ_ENCODING_FORMAT, "8");
            
                //this will be a tgz file for all forms - application files are set to 1
                if (form_file_path.ToLower().Contains("ogz"))
                {
                    Result = RBA_API.SetParam(PARAMETER_ID.P62_REQ_UNPACK_FLAG, "0");
                }
                else
                {
                    Result = RBA_API.SetParam(PARAMETER_ID.P62_REQ_UNPACK_FLAG, "1");
                }

                Result = RBA_API.SetParam(PARAMETER_ID.P62_REQ_FAST_DOWNLOAD, "1");
                Result = RBA_API.SetParam(PARAMETER_ID.P62_REQ_OS_FILE_NAME, form_file_path);
                Result = RBA_API.SetParam(PARAMETER_ID.P62_REQ_FILE_NAME, filename);
                
                Result = RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, "13");
                Result = RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, "11");
                Result = RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, "1");
                Result = RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

                Result = RBA_API.SetParam(PARAMETER_ID.P62_REQ_LAST_MESSAGE_TIMEOUT_SEC, "360");

                ShowMessage("Device Form Loading ...");

                Result = RBA_API.ProcessMessage(MESSAGE_ID.FILE_WRITE);
                Debug.WriteLine("DEVICE UPDATER: SetParam() FILE_WRITE RESULT-----------={0}", Result);
            }
            catch(Exception e)
            {
                Debug.WriteLine("DeviceConfig: EXCEPTION={0}", (object)e.Message);
            }

            return Result;
        }

        public string Get24RebootState()
        {
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_GROUP_NUM, "0007");
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_INDEX_NUM, "0045");
            RBA_API.ProcessMessage(MESSAGE_ID.M61_CONFIGURATION_READ);

            string status = RBA_API.GetParam(PARAMETER_ID.P61_RES_DATA_CONFIG_PARAMETER);

            return status;
        }

        public ERROR_ID Set24RebootState(string mode)
        {
            ERROR_ID result = WriteConfiguration("0007", "0045", mode);
            return result;
        }

        public string Get24RebootTime()
        {
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_GROUP_NUM, "0007");
            RBA_API.SetParam(PARAMETER_ID.P61_REQ_INDEX_NUM, "0046");
            RBA_API.ProcessMessage(MESSAGE_ID.M61_CONFIGURATION_READ);

            string status = RBA_API.GetParam(PARAMETER_ID.P61_RES_DATA_CONFIG_PARAMETER);

            return status;
        }

        public string GetLastRebootFlag()
        {
            string rebootValue = "UNKNOWN";
            try
            {
                string file_path = @"/HOST/MANAGER.DIA";
                string result = RetrieveFile(file_path);

                //parse Manager.DIA file - ; delimited list
                List<string> managerDia = new List<string>();

                //clean up the data
                result = result.Replace("\r\n", "");

                managerDia.AddRange(result.Split(';'));

                string rebootConfig = managerDia.FirstOrDefault(m => m.Contains("020649"));

                if (!string.IsNullOrWhiteSpace(rebootConfig))
                {
                    rebootValue = rebootConfig.Split('=')[1];
                }
            }
            catch (Exception ex)
            {
                OnNotification(this, new NotificationEventArgs { NotificationType = NotificationType.Log, Message = $"Error getting file from device {ex.Message}{Environment.NewLine}" });
            }
            return rebootValue;
        }

        public void Set24RebootTime(string action)
        {
            //check device configuration
            //this will pull the requested file from the device and store in the application executing directory
            //DataFormats 0 = plain text / 1 = Base64 format
            try
            {
                string file_path = @"/HOST/MANAGER.DIA";
                string result = RetrieveFile(file_path);

                //parse Manager.DIA file - ; delimited list
                List<string> managerDia = new List<string>();

                //clean up the data
                result = result.Replace("\r\n", "");

                managerDia.AddRange(result.Split(';'));

                string rebootConfig = managerDia.FirstOrDefault(m => m.Contains("020649"));

                if (!string.IsNullOrWhiteSpace(rebootConfig))
                {
                    string rebootValue = rebootConfig.Split('=')[1];
                    if (rebootValue == "1")
                    {
                        string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                        var directory = System.IO.Path.GetDirectoryName(path);
                        ERROR_ID updateResult = UpdateDevice(Path.Combine(directory, "Resources", Path.GetFileName("MANAGER.PAR")));
                        if (updateResult != ERROR_ID.RESULT_SUCCESS)
                        {
                            OnNotification(this, new NotificationEventArgs { NotificationType = NotificationType.Log, Message = $"Not able to load MANAGER.PAR file to the device.{ Environment.NewLine }"});
                        }
                    }
                }
                //reconfigure device with configuration setting
                SetRebootTime(action);
            }
            catch (Exception ex)
            {
                OnNotification(this, new NotificationEventArgs { NotificationType = NotificationType.Log, Message = $"Error getting file from device {ex.Message}{Environment.NewLine}" });
            }
        }

        private void SetRebootTime(string action)
        {
            bool mode = true;
            string timeStamp = "";

            // action can be either: a) True/False, b) HHMM
            if (action.IndexOf("True", StringComparison.InvariantCultureIgnoreCase) != -1 ||
                action.IndexOf("False", StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                mode = bool.Parse(action);
                Console.WriteLine("\r\nENABLE 24 HOUR REBOOT TEST ----------------------");

                // Check is enabled
                bool enabled =Convert.ToBoolean(Convert.ChangeType(Get24RebootState(), typeof(uint)));
                if(enabled != mode)
                {
                    Set24RebootState(mode ? "1" : "0");
                    enabled = Convert.ToBoolean(Convert.ChangeType(Get24RebootState(), typeof(uint)));
                    Console.WriteLine($"24 HOUR REBOOT OPTION ENABLED  : {enabled}");
                    if(!mode)
                    {
                        ERROR_ID result = WriteConfiguration("0007", "0046", "0");
                        Debug.WriteLine($"WriteConfiguration result      : {result}");
                    }
                    Offline();
                    HardDeviceReset();
                }
                else
                {
                    Console.WriteLine($"24 HOUR REBOOT OPTION ENABLED  : {enabled}");
                }
                return;
            }
            else
            {
                timeStamp = action;
            }

            string deviceRebootTime = Get24RebootTime();
            string configRebootTime = timeStamp;
            //string configRebootTime = DateTime.Now.AddMinutes(5).ToString("HHmm");

            //if the time needs to be set - then the device must reboot so that it stored the updated configuration
            if (deviceRebootTime != configRebootTime)
            {
                Console.WriteLine("\r\nENABLE 24 HOUR REBOOT TEST ----------------------");

                // Check is enabled
                bool enabled =Convert.ToBoolean(Convert.ChangeType(Get24RebootState(), typeof(uint)));
                if(enabled != mode)
                {
                    Set24RebootState(mode ? "1" : "0");
                }
                enabled = Convert.ToBoolean(Convert.ChangeType(Get24RebootState(), typeof(uint)));
                Console.WriteLine($"24 HOUR REBOOT OPTION ENABLED  : {enabled}");

                ERROR_ID result = WriteConfiguration("0007", "0046", configRebootTime);
                Debug.WriteLine($"WriteConfiguration result      : {result}");

                // SAVE UPDATED REBOOT TIME
                deviceRebootTime = Get24RebootTime();
                
                result = Offline();
                Console.WriteLine($"Offline result                 : {result}");

                result = HardDeviceReset();
                Console.WriteLine($"HardDeviceReset result         : {result}");

                //notify DAL that the device is disconnected
                //NotificationRaise(new NotificationEventArgs { NotificationType = NotificationType.DeviceEvent, DeviceEvent = DeviceEvent.DeviceDisconnected });
                Console.WriteLine($"DEVICE DAILY REBOOT TIME       : {deviceRebootTime}\r\n");
            }
        }

        public ERROR_ID WriteConfiguration(string group, string index, string data)
        {
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_GROUP_NUM, group);
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_INDEX_NUM, index);
            RBA_API.SetParam(PARAMETER_ID.P60_REQ_DATA_CONFIG_PARAM, data);
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M60_CONFIGURATION_WRITE);

            return result;
        }

        public ERROR_ID Offline()
        {
             ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M00_OFFLINE);
            return result;
        }

        public string RetrieveFile(string filename)
        {
            RBA_API.SetParam(PARAMETER_ID.P65_REQ_RECORD_TYPE, "1");
            //dataformat - 0 = plain text and 1 = BASE64 Format - all should be plain text 0
            RBA_API.SetParam(PARAMETER_ID.P65_REQ_DATA_TYPE, "0");
            RBA_API.SetParam(PARAMETER_ID.P65_REQ_FILE_NAME, filename);
            ERROR_ID result = RBA_API.ProcessMessage(MESSAGE_ID.M65_RETRIVE_FILE);

            //get value that will determine is the file was actually found and retreived
            string resultMessage = RBA_API.GetParam(PARAMETER_ID.P65_RES_RESULT);
            string fileData = RBA_API.GetParam(PARAMETER_ID.P65_RES_DATA);

            return fileData;
        }

        public string GetVariable_29(string varId)
        {
            RBA_API.SetParam(PARAMETER_ID.P29_REQ_VARIABLE_ID, varId);
            RBA_API.ProcessMessage(MESSAGE_ID.M29_GET_VARIABLE);

            string status = RBA_API.GetParam(PARAMETER_ID.P29_RES_STATUS);
            string variable = RBA_API.GetParam(PARAMETER_ID.P29_RES_VARIABLE_ID);
            string value = RBA_API.GetParam(PARAMETER_ID.P29_RES_VARIABLE_DATA);
            return value;
        }
    }
}
