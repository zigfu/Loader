using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;

namespace LoaderLib2
{
    public class UberLib
    {
        OpenNIListener listener;
        FullscreenDetector fullscreen;
        Thread WindowThread;
        volatile bool waitingForWave = false;
        EventHandler<OpenNI.GestureRecognizedEventArgs> callback;

        public UberLib()
        {
            listener = new OpenNIListener();

            
            callback = new EventHandler<OpenNI.GestureRecognizedEventArgs>(Gesture_GestureRecognized);
            listener.Gesture.GestureRecognized += callback;
            WindowThread = new Thread(ThreadProc);
            WindowThread.IsBackground = true;
            WindowThread.Start();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin,
           uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32.dll")]
        static extern IntPtr DispatchMessage([In] ref NativeMessage lpmsg);
        [DllImport("user32.dll")]
        static extern bool TranslateMessage([In] ref NativeMessage lpMsg);
        private void ThreadProc()
        {
            fullscreen = new FullscreenDetector();
            ShowWindow(fullscreen.hWnd, 4); //4 == SW_SHOWNOACTIVE

            NativeMessage msg;
            while (true) {
                if (PeekMessage(out msg,
                    fullscreen.hWnd,
                    0,
                    0,
                    1 //PM_REMOVE
                    )) {
                        TranslateMessage(ref msg);
                        DispatchMessage(ref msg);
                }
                if (waitingForWave) {
                    listener.ListenOneFrame();
                }
            }
        }


        // TODO: move to separate file - we need this only on windows
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        Process runningProcess;
        // TODO: additional abstraction layer (this is windows-specific)
        public void LaunchProcess(string command, string workingDir)
        {
            ShitOn();
            var CurrentProcess = Process.GetCurrentProcess();
            runningProcess = null;
            ShowWindow(CurrentProcess.MainWindowHandle, 11); // magic number = SW_FORCEMINIMIZE

            //TODO: ugh (change to event instead of polling)
            while (fullscreen.FullscreenAppOpen) {
                System.Threading.Thread.Sleep(100);
            }

            runningProcess = System.Diagnostics.Process.Start(command);
            runningProcess.EnableRaisingEvents = true;
            runningProcess.Exited += new EventHandler(delegate(object sender, EventArgs e) {
                ShowWindow(CurrentProcess.MainWindowHandle, 9); // magic number = SW_SHOW
                Console.WriteLine("removing gesture callback - happy flow");
                ShitOff();
            });
        }

        private void ShitOn()
        {
            waitingForWave = true;
            Console.WriteLine("what a piece of crap");
        }

        private void ShitOff()
        {
            waitingForWave = false;
            Console.WriteLine("not a shit");
        }

        void Gesture_GestureRecognized(object sender, OpenNI.GestureRecognizedEventArgs e)
        {
            Console.WriteLine("shit on a stick");
            if ((null != runningProcess) && (!runningProcess.HasExited)) {
                runningProcess.Kill();
            }
        }
    }
}
