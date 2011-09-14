using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ZigLib
{
    class ZigDB
    {
        string RootDir;

        public Dictionary<string, InstalledZig> Zigs { get; private set; }

        public ZigDB(string ZigDir, string RemoteZigURL)
        {
            RootDir = ZigDir;
            // TODO: provide a way to order items
            Zigs = new Dictionary<string, InstalledZig>();
            foreach (string dir in Directory.GetDirectories(ZigDir)) {
                InstalledZig iz = new InstalledZig(dir);
                Zigs[iz.Metadata.Name] = iz;
            }

            //TODO: remote part of things
        }

        public IEnumerable<InstalledZig> EnumerateInstalledZigs()
        {
            return Zigs.Values;
        }

        public IEnumerable<RemoteZig> EnumerateRemoteZigs()
        {
            return new List<RemoteZig>(); //TODO: real implementation!
        }

        public void InstallZig(RemoteZig zig)
        {
            //TODO:!
        }
    }
}
