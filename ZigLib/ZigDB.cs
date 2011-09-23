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
        string RootURL;

        public Dictionary<string, InstalledZig> zigsByName;
        public Dictionary<string, InstalledZig> zigsByID;

        public ZigDB(string ZigDir, string APIBaseURL)
        {
            RootDir = ZigDir;
            RootURL = APIBaseURL;
            // TODO: provide a way to order items
            zigsByID = new Dictionary<string, InstalledZig>();
            zigsByName = new Dictionary<string, InstalledZig>();
            try {
                foreach (string dir in Directory.GetDirectories(ZigDir)) {
                    InstalledZig iz = new InstalledZig(dir);
                    AddInstalledZig(iz); //TODO: use metadata ID or something like that
                }
            }
            catch (DirectoryNotFoundException) {
                //zig dir doesn't exist - create empty zig dir
                Directory.CreateDirectory(RootDir);
            }
        }

        private void AddInstalledZig(InstalledZig iz)
        {
            zigsByName[iz.Metadata.Name] = iz;
            if (HasValidID(iz)) {
                zigsByID[iz.Metadata.ZigID] = iz;
            }
        }

        private static bool HasValidID(IZig iz)
        {
            return (null != iz.Metadata.ZigID) && ("" != iz.Metadata.ZigID);
        }

        public IEnumerable<InstalledZig> EnumerateInstalledZigs()
        {
            return zigsByName.Values;
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

        public bool IsInstalled(IZig zig)
        {
            if (zigsByName.ContainsKey(zig.Metadata.Name)) {
                return true;
            }
            if (HasValidID(zig) && (zigsByID.ContainsKey(zig.Metadata.ZigID))) {
                return true;
            }
            return false;
        }

        public InstalledZig GetLocalZig(RemoteZig remote)
        {
            // TODO: error handling?
            try {
                return zigsByID[remote.Metadata.ZigID];
            }
            catch (KeyNotFoundException) {
                try {
                    return zigsByName[remote.Metadata.Name];
                }
                catch (KeyNotFoundException) {
                    return null;
                }
            }
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
            AddInstalledZig(iz);
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

        public string GetRemoteZigsQuery()
        {
            return RootURL + "/everyzig/" + OSFilter.AutodetectOS().OsString;
        }

        public string GetZigQuery(string ZigID)
        {
            return RootURL + "/onezig?zigid=" + ZigID;
        }

        public string GetZigQuery(InstalledZig Zig)
        {
            return GetZigQuery(Zig.Metadata.ZigID);
        }

        internal void RemoveZig(InstalledZig zigToRemove)
        {
            if (!IsInstalled(zigToRemove)) {
                //TODO: some error and/or real logging
                Console.Error.WriteLine("attempted to remove missing zig");
            }
            //TODO: real code
        }
    }
}
