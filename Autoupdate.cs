using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace Loader
{

    class Autoupdate
    {
        static string url = "http://zigfu.com/selfupdater.exe";
        static string tempName = "shit.crap";
        static VersionString Version = new VersionString(0, 1, 2, 3);



        private static VersionString GetVersionFromServer(string uri)
        {
            WebRequest get = WebRequest.Create(uri);
            WebResponse result = get.GetResponse();
            string version = (new StreamReader(result.GetResponseStream())).ReadToEnd();
            return new VersionString(version);
        }

        public static void DoAutoupdate()
        {
            VersionString ver = GetVersionFromServer("http://zigfu.com/version.txt");
            //TODO: change WriteLine to log instead of console
            Console.WriteLine("current version {0}, server version {1}", Version, ver);
            if (Version.CompareTo(ver) >= 0) {
                Console.WriteLine("current version {0} newer than server version {1}", Version, ver);
                return;
            }
            Console.WriteLine("doing update!");
            WebRequest get = WebRequest.Create(url);
            WebResponse result = get.GetResponse();
            using (Stream file = result.GetResponseStream()) {
                using (FileStream f = File.Create(tempName)) {
                    file.CopyTo(f);
                }
            }

            string exeName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            if (File.Exists(exeName + ".old")) {
                File.Delete(exeName + ".old");
            }
            File.Move(exeName, exeName + ".old");
            File.Move(tempName, exeName);
            System.Diagnostics.Process.Start(exeName);
        }

    }
}
