using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenNI;

namespace LoaderLib2
{
    internal class OpenNIListener
    {
        string LicenseKey = "0KOIk2JeIBYClPWVnMoRKn5cdY4=";
        string LicenseVendor = "PrimeSense";
        Context context;

        private ProductionNode openNode(NodeType nt)
        {
            if (null == context) return null;

            ProductionNode ret = null;
            try {
                ret = context.FindExistingNode(nt);
            }
            catch {
                ret = context.CreateAnyProductionTree(nt, null);
                Generator g = ret as Generator;
                if (null != g) {
                    g.StartGenerating();
                }
            }
            return ret;
        }

        DepthGenerator Depth;
        public GestureGenerator Gesture { get; private set; }
        UserGenerator Users;
        Thread ListenerThread;
        public OpenNIListener()
        {
            context = new Context();
            License ll = new License();
            ll.Key = LicenseKey;
            ll.Vendor = LicenseVendor;
            context.AddLicense(ll);

            Depth = openNode(NodeType.Depth) as DepthGenerator;
            Gesture = openNode(NodeType.Gesture) as GestureGenerator;
            Users = openNode(NodeType.User) as UserGenerator;

            Gesture.AddGesture("Wave");
            //Gesture.AddGesture("Click");
        }

        public void ListenOneFrame()
        {
            context.WaitAndUpdateAll();
        }
    }
}
