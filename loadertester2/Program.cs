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

        static private ProductionNode openNode(Context OpenNIContext, NodeType nt)
        {
            if (null == OpenNIContext) return null;

            ProductionNode ret = null;
            try {
                ret = OpenNIContext.FindExistingNode(nt);
            }
            catch {
                ret = OpenNIContext.CreateAnyProductionTree(nt, null);
                Generator g = ret as Generator;
                if (null != g) {
                    g.StartGenerating();
                }
            }
            return ret;
        }

        static void Main(string[] args)
        {
            Context context = new Context();
            License ll = new License();
            ll.Key = LicenseKey;
            ll.Vendor = LicenseVendor;
            context.AddLicense(ll);
            DepthGenerator dg = openNode(context, NodeType.Depth) as DepthGenerator;
            dg.StartGenerating();
            GestureGenerator Gesture = openNode(context, NodeType.Gesture) as GestureGenerator;
            Gesture.AddGesture("Wave");
            //while (true) {
                context.WaitAndUpdateAll();
            //}
            //UberLib ul = new UberLib();
            System.Threading.Thread.Sleep(5000);
            //ul.LaunchProcess(@"c:\windows\system32\notepad.exe", "");
            bool done1 = false;
            API.LaunchProcess(@"D:\work\temp\fluidwall-bin-win32-v1.0.1 - (withoutmotor)\fluidWall.exe", @"D:\work\temp\fluidwall-bin-win32-v1.0.1 - (withoutmotor)", context, delegate(object s, EventArgs e) {
                done1 = true;
            });
            while (!done1) {
                System.Threading.Thread.Sleep(200);
            }

            Console.WriteLine("quit 1");
            //API.LaunchProcess(@"D:\work\launchprocesstest\dummyfullscreen.exe", "", context, false);
            //Console.WriteLine("quit 2");
            //API.LaunchProcess(@"D:\work\launchprocesstest\dummyfullscreen.exe", "", context, false);
            //Console.WriteLine("quit 3");
            Console.ReadLine();
        }
    }
}
