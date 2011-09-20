//
// NOTE: To whoever's maintaining this code - Sorry (especially if it's me)
//
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Ionic.Zip;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace Loader
{
    class FullscreenNotifier : Form
    {


        // tray icon
        NotifyIcon TrayIcon;
        System.ComponentModel.IContainer container;

        private uint CallBackMessage;

        public FullscreenNotifier()
        {
            FullscreenAppOpen = false;
            // make the window not appear in the task bar, set it hopefully out of the screen
            // (will clean this up to make it really invisible sometime in the future)
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Location = new System.Drawing.Point(-4000, -4000);
            Size = new System.Drawing.Size(1, 1); 

            this.Load += new EventHandler(FullscreenNotifier_Loaded);

            container = new System.ComponentModel.Container();

            TrayIcon = new NotifyIcon(container);
            TrayIcon.Visible = true;
            TrayIcon.Icon = new System.Drawing.Icon("Ninja-Toy.ico");
            TrayIcon.Text = "ZigFu loader. Having this means you're awesome!";
            TrayIcon.ContextMenuStrip = new ContextMenuStrip(container) {
                Items = {
                    new ToolStripLabel("E&xit", null, false, new EventHandler(delegate(object o, EventArgs e) { TrayIcon.Visible = false;  this.Close(); }))
                },
                
            };
            // TODO: left click gives context menu too
        }

        void FullscreenNotifier_Loaded(object sender, EventArgs e)
        {
            APPBARDATA appBar = new APPBARDATA();
            appBar.cbSize = Marshal.SizeOf(appBar);
            appBar.hWnd = this.Handle;

            // Define a new window message that is guaranteed to be 
            // unique throughout the system.
            const string Message = "ShitReceiveFullScreenNotification";
            CallBackMessage = NativeMethod.RegisterWindowMessage(Message);
            if (CallBackMessage == 0) {
                throw new Exception(":(");
            }

            // Register a new appbar and specifies the message identifier 
            // that the system should use to send it notification 
            // messages. The call returns FALSE if an error occurs or if 
            // the full screen notification is already enabled.
            appBar.uCallbackMessage = CallBackMessage;


            if (NativeMethod.SHAppBarMessage(ABMsg.ABM_NEW, ref appBar) == 0) {
                throw new Exception("SHAppBarMessage failed");
            }
        }

        public bool FullscreenAppOpen { get; private set; }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == this.CallBackMessage) {
                if (m.WParam.ToInt32() == (int)ABNotify.ABN_FULLSCREENAPP) {
                    FullscreenAppOpen = (m.LParam != IntPtr.Zero);

                    Console.WriteLine("A full-screen application is " +
                        (FullscreenAppOpen ? "opening" : "closing"));
                }
            }
            base.WndProc(ref m);
        }
    }

    class Program
    {


        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int AttachThreadInput(int idAttach, int idAttachTo, int fAttach);

        static void Main(string[] args)
        {
            //testAutoUpdate(args);


            //ExtractZip("temp.zip");

            //testVersionObject();


            System.Diagnostics.Process proc = null;
            if (args.Length > 0) {
                string RootProgram = args[0];
                if (RootProgram == "client") {
                    Console.WriteLine("Client! Invoking!");
                    LoaderLib.LoaderAPI.ConnectToServer().LaunchProcess(@"c:\windows\system32\notepad.exe", "shit");
                    return;
                }
                Console.WriteLine("Server! Use Ctrl-C to exit!");
                Console.WriteLine("Running Launcher: {0}", RootProgram);
                proc = System.Diagnostics.Process.Start(RootProgram);
            }
            else {
                Console.WriteLine("Server! Use Ctrl-C to exit!");
                Console.WriteLine("Assuming launcher already running");
            }

            FullscreenNotifier f = new FullscreenNotifier();
            f.Show();

            var SharedObject = LoaderLib.LoaderAPI.StartServer(f.Handle, delegate(object sender, LoaderLib.CreateProcessEventArgs e) {
                Console.WriteLine("CreateProcess: {0}, in dir: {1}", e.Command, e.Path);
                if (proc != null) {
                    ShowWindow(proc.MainWindowHandle, 11); // magic number = SW_FORCEMINIMIZE
                }

                //TODO: ugh (change to event instead of polling)
                while (f.FullscreenAppOpen) {
                    System.Threading.Thread.Sleep(100);
                }

                var newProc = System.Diagnostics.Process.Start(e.Command);
                newProc.EnableRaisingEvents = true;
                newProc.Exited += new EventHandler(delegate(object sender2, EventArgs e2) {
                    if (proc != null) {
                        ShowWindow(proc.MainWindowHandle, 9); // magic number = SW_SHOW
                    }
                });
                Console.WriteLine("done with callback");

            });

            f.HandleCreated += delegate(object s, EventArgs e) { SharedObject.ServerWindowHandle = f.Handle; };

            Application.Run(f); 

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
