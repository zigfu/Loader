using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using Ionic.Zip;

namespace ZigLib
{
    public class OSFilter
    {
        public string OsString { get; private set; }

        private OSFilter(string OsString)
        {
            this.OsString = OsString;
        }
        public static OSFilter Windows = new OSFilter("win");
        public static OSFilter Mac = new OSFilter("mac");
        public static OSFilter All = new OSFilter("all");

        public static OSFilter AutodetectOS()
        {
            if (System.Environment.OSVersion.Platform == PlatformID.Win32NT) {
                return Windows;
            }
            else {
                //TODO: test out other OSes maybe?
                return Mac;
            }
        }
    }

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
                    Zigs[iz.Metadata.Name] = iz; //TODO: use metadata ID or something like that
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
            foreach (object entry in (ArrayList)ht["zigEntries"]) {
                output.Add(new RemoteZig(entry as Hashtable));
            }
            //TODO: remove installed zigs from the list according to some ID parameter
            return output; 
        }

        public bool IsInstalled(RemoteZig zig)
        {
            return Zigs.ContainsKey(zig.Metadata.Name); //TODO: change on adding ID to zigs
        }
        
        public InstalledZig InstallZig(string PathToZigFile)
        {
            //TODO: Make sure it's a valid zig file (extract metadata, see that it's okay)
            //TODO: get directory name from metadata
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
