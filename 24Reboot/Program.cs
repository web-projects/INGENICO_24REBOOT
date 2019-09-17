using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceConfig
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"\r\n==========================================================================================");
            Console.WriteLine($"{Assembly.GetEntryAssembly().GetName().Name} - Version {Assembly.GetEntryAssembly().GetName().Version}");
            Console.WriteLine($"==========================================================================================\r\n");

            if(args.Length > 0)
            {
                switch(args[0])
                {
                    case "/SET":
                    {
                        if(args.Length == 3)
                        { 
                            DeviceConfig updater = new DeviceConfig(args[1]);
                            updater.configDevice(args[2]);
                        }
                        else
                        {
                            Console.WriteLine("24HOUR REBOOT [/SET]: Missing Parameter - [PORT] [MODE] | [TIME]");
                        }
                        break;
                    }

                    case "/GET":
                    {
                        if(args.Length == 2)
                        { 
                            DeviceConfig updater = new DeviceConfig(args[1]);
                            updater.configRead();
                        }
                        else
                        {
                            Console.WriteLine("24HOUR REBOOT [/GET]: Missing Parameter - [PORT]");
                        }
                        break;
                    }

                    default:
                    {
                        Console.WriteLine("24HOUR REBOOT: Missing Verb - [/GET] [/SET]");
                        break;
                    }
                }
            }
            else
            {
                Console.WriteLine("24HOUR REBOOT: Missing Verb - [/GET] [/SET]");
            }
        }
    }
}
