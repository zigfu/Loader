using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

namespace ZigLib
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
                Zigs[iz.Metadata.Name] = iz;
            }
        }

        public IEnumerable<InstalledZig> EnumerateInstalledZigs()
        {
            return Zigs.Values;
        }

        public IEnumerable<RemoteZig> EnumerateRemoteZigs(string zigsJson)
        {
            Hashtable ht = (Hashtable)JSON.JsonDecode(zigsJson);
            List<RemoteZig> output = new List<RemoteZig>();
            foreach (object entry in (ArrayList)ht["zigs"]) {
                output.Add(new RemoteZig(entry as Hashtable));
            }
            return output; //TODO: real implementation!
        }

        public void InstallZig(RemoteZig zig)
        {
            //TODO:!
        }
    }
}
