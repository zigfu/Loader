using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using Ionic.Zip;

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
            try {
                foreach (string dir in Directory.GetDirectories(ZigDir)) {
                    InstalledZig iz = new InstalledZig(dir);
                    Zigs[iz.Metadata.Name] = iz;
                }
            }
            catch (DirectoryNotFoundException) {
                //zig dir doesn't exist - create empty zig dir
                Directory.CreateDirectory(RootDir);
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
        
        public InstalledZig InstallZig(string PathToZigFile)
        {
            //TODO: Make sure it's a valid zig file (extract metadata, see that it's okay)
            //TODO: check for overwrites!
            string OutDir = Path.Combine(RootDir, Path.GetFileName(PathToZigFile));
            CreateDirRecursive(new DirectoryInfo(OutDir));
            ExtractZip(PathToZigFile, OutDir);
            InstalledZig iz = new InstalledZig(OutDir);
            Zigs[iz.Metadata.Name] = iz;
            return iz;
        }

        private static void CreateDirRecursive(DirectoryInfo inf) {
            if (inf.Parent != null) {
                CreateDirRecursive(inf.Parent);
            }
            if (!inf.Exists) {
                inf.Create();
            }
        }

        private static void ExtractZip(string InPath, string OutPath)
        {
            using (ZipFile z = ZipFile.Read(InPath)) {
                z.ExtractAll(OutPath, ExtractExistingFileAction.OverwriteSilently); // TODO: handle errors?
            }
        }
    }
}
