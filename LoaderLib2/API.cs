using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoaderLib2
{
    public class API
    {
        internal static UberLib _Uber;
        internal static UberLib Uber
        {
            get
            {
                if (null == _Uber) {
                    _Uber = new UberLib();
                    _Uber.WaitTillInit();
                }
                return _Uber;
            }
        }


        static public void LaunchProcess(string Command, string WorkingDirectory, OpenNI.Context Context, EventHandler DoneCallback)
        {
            if (Uber != null) {
                Console.WriteLine(":)");
                //throw new Exception("hurrah");
                Uber.LaunchProcess(Command, WorkingDirectory, Context, DoneCallback);
            }
        }

        static public void Shutdown()
        {
            if (_Uber != null) {
                Uber.Shutdown();
                _Uber = null;
            }

        }
    }
}
