using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
namespace ZigLib
{
    public class InstalledZig : IZig
    {
        const string MetadataFilename = ".metadata";
        public ZigMetadata Metadata { get; private set; }

        public string InstallPath { get; private set; }
        public string RunCommand {get; private set; }
        //TODO: cmdline-arguments

        const string COMMAND = "command";

        public InstalledZig(string InstallPath)
        {
            Hashtable props = (Hashtable)JSON.JsonDecode(File.ReadAllText(Path.Combine(InstallPath, MetadataFilename)));
            //TODO: make sure the thumbnail points to disk instead of web URL
            this.Metadata = new ZigMetadata(props);
            this.RunCommand = (string)props[COMMAND];
            this.InstallPath = InstallPath;
        }

        //TODO: really, really ugh
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public void Launch()
        {
            var RemoteAPI = LoaderLib.LoaderAPI.ConnectToServer();
            SetForegroundWindow(RemoteAPI.ServerWindowHandle);
            string workingDir = Path.GetFullPath(InstallPath);
            //TODO: make RunCommand stay relative (some minor changes to LoaderAPI required)
            RemoteAPI.LaunchProcess(Path.Combine(workingDir, RunCommand), workingDir);
        }

        public override string ToString()
        {
            return string.Format("InstalledZig {0}, Command: {1}", Metadata.Name, RunCommand);
        }
    }
}
