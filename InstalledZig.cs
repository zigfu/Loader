using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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

        public void Launch()
        {
            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo(md.Command);
            psi.WorkingDirectory = InstallPath;
            //Console.WriteLine("Take off every zig!");
            var process = System.Diagnostics.Process.Start(psi);
            process.WaitForExit();
            //Console.WriteLine("done with process");
        }
    }
}
