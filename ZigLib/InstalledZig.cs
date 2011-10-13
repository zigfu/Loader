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

        public string InstallPath { get; private set; }
        public string RunCommand {get; private set; }
        public SharedMetadata Metadata { get; private set; }
        public string ThumbnailURI { get; private set; }
        public string WorkingDir { get; private set; }
        //TODO: cmdline-arguments


        const string MetadataFilename = ".metadata";

        //private static string EncodePath(string fullpath)
        //{
        //    List<string> parts = new List<string>();
        //    //TODO (WARNING): this routine assumes the given path is an absolute path to a local file!
        //    string currentRemaining = fullpath;
        //    string currentFile = Path.GetFileName(fullpath);
        //    while ((currentRemaining != null) && (currentRemaining.Length > 3)) {
        //        currentFile = Path.GetFileName(currentRemaining);
        //        currentRemaining = Path.GetDirectoryName(currentRemaining);
        //        parts.Insert(0, System.Web.HttpUtility.UrlPathEncode(currentFile));
        //    } 
        //    parts.Insert(0, Path.GetPathRoot(fullpath).Substring(0, 2));
        //    //parts.Add(Path.GetPathRoot(fullpath));
        //    return string.Join("/", parts.ToArray());
        //}

        public InstalledZig(string InstallPath)
        {
            Hashtable props = (Hashtable)JSON.JsonDecode(File.ReadAllText(Path.Combine(InstallPath, MetadataFilename)));
            Metadata = new SharedMetadata(props);
            this.RunCommand = (string)props[ZigProperties.COMMAND]; //relative to Install path
            this.InstallPath = InstallPath;
            if (props.ContainsKey(ZigProperties.WORKING_DIRECTORY)) {
                this.WorkingDir = (string)props[ZigProperties.WORKING_DIRECTORY];
            }
            else {
                // working dir default is the directory where the exe resides
                this.WorkingDir = Path.GetDirectoryName(this.RunCommand);
            }

            // turn thumbnail path from relative path to file:// URI)
            string fulldir = Path.GetFullPath(InstallPath);
            string AbsoluteIconPath = Path.Combine(fulldir, (string)props[ZigProperties.THUMBNAIL_PATH]);

            ThumbnailURI = "file://" + AbsoluteIconPath.Replace('\\', '/');//"file://" + EncodePath(AbsoluteIconPath);
        }

        //TODO: really, really ugh
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public void Launch(OpenNI.Context Context, EventHandler DoneCallback)
        {
            //var RemoteAPI = LoaderLib.LoaderAPI.ConnectToServer();
            //if (null == RemoteAPI) {
            //    return; //TODO: launch loader.exe or return some error instead of silent fail
            //}
            //SetForegroundWindow(RemoteAPI.ServerWindowHandle);
            string workingDir = Path.Combine(Path.GetFullPath(InstallPath), this.WorkingDir);
            ////TODO: make RunCommand stay relative (some minor changes to LoaderAPI required)
            //RemoteAPI.LaunchProcess(Path.Combine(workingDir, RunCommand), workingDir, System.Diagnostics.Process.GetCurrentProcess().Id);
            LoaderLib2.API.LaunchProcess(Path.Combine(workingDir, RunCommand), workingDir, Context, DoneCallback);
        }

        public override string ToString()
        {
            return string.Format("InstalledZig {0}, Command: {1}", Metadata.Name, RunCommand);
        }
    }
}
