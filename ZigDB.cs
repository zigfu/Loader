using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Loader
{
    class ZigDB
    {
        string RootDir;

        public Dictionary<string, InstalledZig> Zigs { get; private set; }

        public ZigDB(string ZigDir)
        {
            RootDir = ZigDir;
            // TODO: provide a way to order items
            Zigs = new Dictionary<string, InstalledZig>();
            foreach (string dir in Directory.GetDirectories(ZigDir)) {
                InstalledZig iz = new InstalledZig(dir);
                Zigs[iz.Name] = iz;
            }
        }
    }
}
