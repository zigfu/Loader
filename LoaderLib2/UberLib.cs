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
        //FullscreenDetector fullscreen;
        //Thread WindowThread;
        volatile bool waitingForWave = false;
        EventHandler<OpenNI.GestureRecognizedEventArgs> callback;

        EventWaitHandle InitedEvent;
        Context OpenNIContext;
        GestureGenerator Gesture;
        WindowFinder HwndEnumerator;
        public UberLib()
        {
            //TODO: set true somewhere else
            //InitedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

            callback = new EventHandler<OpenNI.GestureRecognizedEventArgs>(Gesture_GestureRecognized);
            
            //WindowThread = new Thread(ThreadProc);
            //WindowThread.Start();
            //WindowThread.IsBackground = true;
            HwndEnumerator = new WindowFinder();
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

        //bool quit = false;
        //private void ThreadProc()
        //{
        //    fullscreen = new FullscreenDetector();
        //    InitedEvent.Set();
        //    NativeMessage msg;
        //    while (!quit) {
        //        if (PeekMessage(out msg,
        //            fullscreen.hWnd,
        //            0,
        //            0,
        //            1 //PM_REMOVE
        //            )) {
        //            TranslateMessage(ref msg);
        //            DispatchMessage(ref msg);
        //        }
        //        lock (this) {
        //            if (waitingForWave) {
        //                OpenNIContext.WaitAndUpdateAll();
        //            }
        //        }
        //    }

        //}

        // TODO: move to separate file - we need this only on windows
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        // needed for getting current window handle (MainWindowHandle doesn't work :( )
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentProcessId();
        Process runningProcess;
        // TODO: additional abstraction layer (this is windows-specific)
        public void LaunchProcess(string command, string workingDir, Context context, EventHandler DoneCallback)
        {
            //note: for some reason the fact that this is in a thread is CRITICAL to it working correctly
            // otherwise, the second ShowWindow call (restoring our original window) doesn't have the desired effect
            // this might be because if we don't create the thread then we're not letting the windowproc handle
            // the message till we're done with the function, but I don't really know why. Just that when running
            // from a thread everything works as expected :(
            Thread t = new Thread(delegate() {
                HwndEnumerator.Update();
                //fullscreen.FullscreenAppOpen = FromFullscreen;
                ShitOn(context);
                try {
                    lock (this) {
                        runningProcess = null;
                    }
                    //var CurrentProcess = Process.GetCurrentProcess();
                    //IntPtr WindowHandle = CurrentProcess.MainWindowHandle;
                    IntPtr WindowHandle = HwndEnumerator.MainWindowOfPid((int)GetCurrentProcessId());
                    Console.WriteLine("asdf1: " + WindowHandle);
                    ShowWindow(WindowHandle, 11); // magic number = SW_FORCEMINIMIZE
                    Console.WriteLine("fdsa");
                    try {
                        //TODO: ugh (change to event instead of polling)
                        //while (fullscreen.FullscreenAppOpen) {
                        while (MyClass.isFullscreen(WindowHandle)) {
                            System.Threading.Thread.Sleep(100);
                        }
                        Console.WriteLine("hjkl");
                        lock (this) {
                            runningProcess = Process.Start(command);
                        }
                        while (!runningProcess.HasExited) {
                            OpenNIContext.WaitAndUpdateAll();
                        }
                    }
                    finally {
                        HwndEnumerator.Update();
                        ShowWindow(WindowHandle, 9); // magic number = SW_RESTORE
                    }
                }
                finally {
                    ShitOff();
                    if (null != DoneCallback) {
                        DoneCallback(this, new EventArgs());
                    }
                }
            });
            t.Start();
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
            lock (this) {
                waitingForWave = true;
            }
            Console.WriteLine("what a piece of crap");
        }

        private void ShitOff()
        {
            lock (this) {
                waitingForWave = false;
            }
            Gesture.RemoveGesture("Wave");
            Gesture.GestureRecognized -= callback;
            Console.WriteLine("not a shit");
        }

        void Gesture_GestureRecognized(object sender, OpenNI.GestureRecognizedEventArgs e)
        {
            Console.WriteLine("shit on a stick");
            lock (this) {
                if ((null != runningProcess) && (!runningProcess.HasExited)) {
                    runningProcess.Kill();
                }
            }
        }

        internal void WaitTillInit()
        {
            //InitedEvent.WaitOne();
        }

        internal void Shutdown()
        {
            //quit = true;
            //fullscreen.Dispose();
        }
    }
}
