using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ZigLib;
using System.IO;
using System.Collections;

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
            RemoteZig remoteLast = null;
            Console.WriteLine("Network Zigs:");
            WebRequest wr = WebRequest.Create(ZigLib.ZigLib.GetRemoteZigsQuery());
            foreach (RemoteZig zig in ZigLib.ZigLib.EnumerateRemoteZigs(new StreamReader(wr.GetResponse().GetResponseStream()).ReadToEnd())) {
                Console.WriteLine(zig);
                remoteLast = zig;
            }
            if (args.Length > 0) {
                if (args[0] == "shit") {
                    if (null != last) {
                        Console.WriteLine("Doing shit (Launching last enumerated zig)");
                        //last.Launch();
                    }
                    else {
                        Console.WriteLine("Sorry, but the princess is in another castle (there are no installed zigs)");
                    }
                }
                else if (args[0] == "crap") {
                    if (null != remoteLast) {
                        Console.WriteLine("Doing crap (Installing last remote zig)");
                        Console.WriteLine("Getting remote data from " + remoteLast.RemoteURI + "to temp.zig");
                        WebRequest wr2 = WebRequest.Create(remoteLast.RemoteURI);
                        using (Stream outStream = File.Create("temp.zig")) {
                            // CopyTo is .net 4 only...
                            wr2.GetResponse().GetResponseStream().CopyTo(outStream);
                        }
                        Console.WriteLine("Done downloading! installing from temp.zig");
                        ZigLib.ZigLib.InstallZig("temp.zig");
                        Console.WriteLine("Really done now!");
                    }
                    else {
                        Console.WriteLine("Sorry, better luck next time (no remote zigs)");
                    }
                }
                else if (args[0] == "urgh") {
                    if (args.Length < 2) {
                        Console.WriteLine("gimme another argument!");
                        return;
                    }
                    else {
                        Console.WriteLine("Checking if zigid {0} is in the remote db", args[1]);
                        string query = ZigLib.ZigLib.GetLocalZigQuery(args[1]);
                        Console.WriteLine("Query: " + query);
                        WebRequest wr3 = WebRequest.Create(query);
                        Console.WriteLine("Response: " + new StreamReader(wr3.GetResponse().GetResponseStream()).ReadToEnd());

                    }
                }
                else if (args[0] == "holy") {
                    if (args.Length < 2) {
                        Console.WriteLine("gimme another argument!");
                        return;
                    }
                    else {
                        //Console.WriteLine("Moving current dir to {0}", args[1]);
                        Console.WriteLine("blah");

                        var di = new DirectoryInfo(ZigLib.Utility.GetMainModuleDirectory());
                        ZigLib.Utility.MoveDirWithLockedFile(di.FullName, Path.Combine(di.Parent.FullName, args[1]));
                    }

                }
                else if (args[0] == "fuck") {
                    if (args.Length < 3) {
                        Console.WriteLine("gimme mo arguments!");
                        return;
                    }
                    else {
                        Console.WriteLine("moving dir {0} to {1}", args[1], args[2]);
                        ZigLib.Utility.MoveDirWithLockedFile(args[1], args[2]);
                    }
                }
                else if (args[0] == "update") {
                    WebRequest GetVersion = WebRequest.Create("http://zigfu.com/portal/version/win");
                    string download_url;
                    double version;
                    using (Stream res = GetVersion.GetResponse().GetResponseStream()) {
                        Hashtable response = JSON.JsonDecode((new StreamReader(res)).ReadToEnd()) as Hashtable;
                        download_url = (string)response["dl_url"];
                        version = (double)response["version"];
                    }
                    Console.WriteLine("Got version response: version {0}, download link {1}", version, download_url);
                    //TODO: check version too
                    WebRequest NewVersion = WebRequest.Create(download_url);
                    const string TEMP_FILE = "portal_temp.zip";
                    //TODO: make sure we delete portal_temp.zip
                    using (Stream output = File.Create(TEMP_FILE)) {
                        NewVersion.GetResponse().GetResponseStream().CopyTo(output);
                    }
                    Console.WriteLine("Done downloading to " + TEMP_FILE);
                    var di = new DirectoryInfo(ZigLib.Utility.GetMainModuleDirectory());
                    string TempExtractedPath = Path.Combine(di.Parent.FullName, "new_version");
                    Console.WriteLine("downloading and extracting zip to " + TempExtractedPath);
                    if (Directory.Exists(TempExtractedPath)) {
                        Console.WriteLine("Dir already exists, emptying it first");
                        Directory.Delete(TempExtractedPath);
                    }
                    Directory.CreateDirectory(TempExtractedPath);
                    //using (var zip = Ionic.Zip.ZipFile.Read(NewVersion.GetResponse().GetResponseStream())) {
                    //    zip.ExtractAll(TempExtractedPath);
                    //}
                    using (var zip = Ionic.Zip.ZipFile.Read(TEMP_FILE)) {
                        zip.ExtractAll(TempExtractedPath);
                    }
                    string TempRunningDir = Path.Combine(di.Parent.FullName, "bin.old");
                    Console.WriteLine("moving current directory ({0}) to {1}", di.FullName, TempRunningDir);
                    ZigLib.Utility.MoveDirWithLockedFile(di.FullName, TempRunningDir);
                    Console.WriteLine("moving temp directory {0} to binary directory {1}", TempExtractedPath, di.FullName);
                    ZigLib.Utility.MoveDirWithLockedFile(TempExtractedPath, di.FullName);
                }
                else {
                    Console.WriteLine("Installing local Zig File: " + args[0]);
                    InstalledZig newZig = ZigLib.ZigLib.InstallZig(args[0]);
                    Console.WriteLine("New Zig: {0}", newZig);
                }
            }

        }
    }
}
