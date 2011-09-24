using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ZigLib
{
    public class Utility
    {

        public static string GetMainModuleDirectory() {
            string MainModulePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            return (new FileInfo(MainModulePath)).DirectoryName;
        }

        // depth-first recursive move directory 
        public static void MoveDirWithLockedFile(string OldDir, string NewDir)
        {
            if (!Directory.Exists(NewDir)) {
                Directory.CreateDirectory(NewDir);
            }
            DirectoryInfo di = new DirectoryInfo(OldDir);
            // call recursiively on subdirs
            foreach (var subdir in di.GetDirectories()) {
                MoveDirWithLockedFile(Path.Combine(OldDir, subdir.Name), Path.Combine(NewDir, subdir.Name));
            }

            foreach (var file in di.GetFiles()) {
                file.MoveTo(Path.Combine(NewDir, file.Name));
            }


            try {
                di.Delete();
            }
            catch (IOException) {
                //TODO: safer behavior instead of silently catching failures...
            }
        }
    }
}
