using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoaderLib2;
using OpenNI;

namespace loadertester2
{
    class Program
    {
        static public string LicenseKey = "0KOIk2JeIBYClPWVnMoRKn5cdY4=";
        static public string LicenseVendor = "PrimeSense";

        static void Main(string[] args)
        {
            Context context = new Context();
            License ll = new License();
            ll.Key = LicenseKey;
            ll.Vendor = LicenseVendor;
            context.AddLicense(ll);
            //UberLib ul = new UberLib();
            System.Threading.Thread.Sleep(1000);
            //ul.LaunchProcess(@"c:\windows\system32\notepad.exe", "");
            API.LaunchProcess(@"D:\work\launchprocesstest\dummyfullscreen.exe", "", context);
            Console.WriteLine("quit 1");
            API.LaunchProcess(@"D:\work\launchprocesstest\dummyfullscreen.exe", "", context);
            Console.WriteLine("quit 2");
            API.LaunchProcess(@"D:\work\launchprocesstest\dummyfullscreen.exe", "", context);
            Console.WriteLine("quit 3");
            Console.ReadLine();
        }
    }
}
