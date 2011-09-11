using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Ionic.Zip;

namespace Loader
{
    class Program
    {
        static void Main(string[] args)
        {
            //testAutoUpdate(args);


            //ExtractZip("temp.zip");

            //testVersionObject();

            //TestMetadata();

            ZigDB zdb = new ZigDB("zigs");
            zdb.Zigs["test"].Launch();
        }

        private static void TestMetadata()
        {
            foreach (string md in Directory.GetFiles(".", "zig_md*.txt")) {
                printMetadata(md);
            }
        }

        private static void printMetadata(string path)
        {
            try {
                Metadata md = Metadata.FromFile(path);
                Console.WriteLine("Loaded Metadata from file " + path);
                Console.WriteLine(md);
            }
            catch (Exception ex) {
                Console.WriteLine("Failed to load Metadata in file {0}. Exception:", path);
                Console.WriteLine(ex);
            }
        }

        private static void testVersionObject()
        {
            VersionString v1 = new VersionString("1.2.3.4");
            VersionString v2 = new VersionString(1, 2, 3, 4);
            VersionString v3 = new VersionString(1, 3, 3, 4);
            VersionString v4 = new VersionString(1, 2, 2, 4);
            if (v1.CompareTo(v2) != 0) {
                Console.WriteLine("1 bad");
            }
            else {
                Console.WriteLine("1 good");
            }
            if (v2.CompareTo(v3) >= 0) {
                Console.WriteLine("2 bad");
            }
            else {
                Console.WriteLine("2 good");
            }
            if (v2.CompareTo(v4) <= 0) {
                Console.WriteLine("3 bad");
            }
            else {
                Console.WriteLine("3 good");
            }
        }

        private static void testAutoUpdate(string[] args)
        {
            if (args.Length == 0) {
                System.Console.WriteLine("dummy run");
                File.WriteAllText("text.txt", "did dummy run");
                return;
            }
            else {
                Autoupdate.DoAutoupdate();
            }
        }

        private static void ExtractZip(string path)
        {
            using (ZipFile z = ZipFile.Read(path)) {
                foreach (var e in z.Entries) {
                    e.Extract(ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }


    }
}
