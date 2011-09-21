using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using ZigLib;
using System.IO;
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
                        last.Launch();
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
                else {
                    Console.WriteLine("Installing local Zig File: " + args[0]);
                    InstalledZig newZig = ZigLib.ZigLib.InstallZig(args[0]);
                    Console.WriteLine("New Zig: {0}", newZig);
                }
            }

        }
    }
}
