using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using OpenNI;
namespace LoaderLib2
{
    //TODO: make internal once everything's working
    public class UberLib
    {
        FullscreenDetector fullscreen;
        Thread WindowThread;
        volatile bool waitingForWave = false;
        EventHandler<OpenNI.GestureRecognizedEventArgs> callback;

        EventWaitHandle InitedEvent;
        Context OpenNIContext;
        GestureGenerator Gesture;

        public UberLib()
        {
            //TODO: set true somewhere else
            InitedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

            callback = new EventHandler<OpenNI.GestureRecognizedEventArgs>(Gesture_GestureRecognized);
            
            WindowThread = new Thread(ThreadProc);
            WindowThread.Start();
            WindowThread.IsBackground = true;
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
            InitedEvent.Set();
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
                    OpenNIContext.WaitAndUpdateAll();
                }
            }
        }

        // TODO: move to separate file - we need this only on windows
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        Process runningProcess;
        // TODO: additional abstraction layer (this is windows-specific)
        public void LaunchProcess(string command, string workingDir, Context context)
        {
            ShitOn(context);
            try {
                var CurrentProcess = Process.GetCurrentProcess();
                runningProcess = null;
                ShowWindow(CurrentProcess.MainWindowHandle, 11); // magic number = SW_FORCEMINIMIZE
                try {
                    //TODO: ugh (change to event instead of polling)
                    while (fullscreen.FullscreenAppOpen) {
                        System.Threading.Thread.Sleep(100);
                    }

                    runningProcess = Process.Start(command);
                    runningProcess.WaitForExit();
                }
                finally {
                    ShowWindow(CurrentProcess.MainWindowHandle, 9); // magic number = SW_SHOW
                }
            }
            finally {
                ShitOff();
            }
        }
        private ProductionNode openNode(NodeType nt)
        {
            if (null == OpenNIContext) return null;

            ProductionNode ret = null;
            try {
                ret = OpenNIContext.FindExistingNode(nt);
            }
            catch {
                ret = OpenNIContext.CreateAnyProductionTree(nt, null);
                Generator g = ret as Generator;
                if (null != g) {
                    g.StartGenerating();
                }
            }
            return ret;
        }
        private void ShitOn(Context context)
        {
            OpenNIContext = context;
            Gesture = openNode(NodeType.Gesture) as GestureGenerator;
            Gesture.AddGesture("Wave");
            Gesture.GestureRecognized += callback;
            waitingForWave = true;
            Console.WriteLine("what a piece of crap");
        }

        private void ShitOff()
        {
            waitingForWave = false;
            Gesture.RemoveGesture("Wave");
            Gesture.GestureRecognized -= callback;
            Console.WriteLine("not a shit");
        }

        void Gesture_GestureRecognized(object sender, OpenNI.GestureRecognizedEventArgs e)
        {
            Console.WriteLine("shit on a stick");
            if ((null != runningProcess) && (!runningProcess.HasExited)) {
                runningProcess.Kill();
            }
        }

        internal void WaitTillInit()
        {
            InitedEvent.WaitOne();
        }
    }
}
