using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZigLib;
namespace ZigTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Installed Zigs:");
            InstalledZig last = null;
            foreach (InstalledZig zig in ZigLib.ZigLib.EnumerateInstalledZigs()) {
                Console.WriteLine(zig);
                last = zig;
            }
            Console.WriteLine("Network Zigs:");
            foreach (RemoteZig zig in ZigLib.ZigLib.EnumerateRemoteZigs()) {
                Console.WriteLine(zig);
            }
            if (!((args.Length > 0) && (args[0] == "shit"))) {
                if (null != last) {
                    Console.WriteLine("Doing shit (Launching last enumerated zig)");
                    last.Launch();
                }
                else {
                    Console.WriteLine("Sorry, but the princess is in another castle (there are no installed zigs)");
                }
            }
        }
    }
}
