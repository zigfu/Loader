using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Loader
{
    class InstalledZig
    {
        const string MetadataFilename = ".metadata";
        public Metadata md { get; private set; }

        private string InstallPath;
        public InstalledZig(string installPath)
        {
            this.InstallPath = installPath;
            md = Metadata.FromFile(Path.Combine(installPath, MetadataFilename));
        }

        public string Name { get { return md.Name; } }

        Process RunningProcess;
        public void KillProcess()
        {
            lock (this) {
                if (RunningProcess != null) {
                    //TODO: send the running process some message to tell it to quit
                    RunningProcess.Kill();
                    RunningProcess = null;
                }
            }
        }


        public void Launch()
        {
            lock (this) {
                if (null != RunningProcess) {
                    return;
                }
                ProcessStartInfo psi = new ProcessStartInfo(md.Command);
                psi.WorkingDirectory = InstallPath;
                //Console.WriteLine("Take off every zig!");
                RunningProcess = Process.Start(psi);
            }
            //TODO: ugly as hell, do better locking mechanism
            var proc = RunningProcess;
            RunningProcess.WaitForExit();
            lock (this) {
                if (proc == RunningProcess) {
                    RunningProcess = null;
                }
            }
            //Console.WriteLine("done with process");
        }


    }
}
