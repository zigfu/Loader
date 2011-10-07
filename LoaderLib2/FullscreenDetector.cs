using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using HWND = System.IntPtr;
using System.ComponentModel;

namespace LoaderLib2
{
    class FullscreenDetector : IDisposable
    {
        [Flags]
        public enum ClassStyles : uint
        {
            ByteAlignClient = 0x1000,
            ByteAlignWindow = 0x2000,
            ClassDC = 0x40,
            DoubleClicks = 0x8,
            DropShadow = 0x20000,
            GlobalClass = 0x4000,
            HorizontalRedraw = 0x2,
            NoClose = 0x200,
            OwnDC = 0x20,
            ParentDC = 0x80,
            SaveBits = 0x800,
            VerticalRedraw = 0x1,
            None = 0x0,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WndClassEx
        {
            public uint cbSize;
            public ClassStyles style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProcDelegate lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        internal delegate long WndProcDelegate(HWND hWnd, uint msg, int wParam, int lParam);

        [DllImport("User32.dll", SetLastError = true)]
        public extern static UInt16 RegisterClassEx(
        [MarshalAs(UnmanagedType.Struct)]ref WndClassEx wndClassEx
        );
 
        [DllImport("User32.dll", SetLastError = true)]
        public extern static bool UnregisterClass(string lpClassName, IntPtr hInstance);
 
        [DllImport("User32.dll")]
        public extern static long DefWindowProc(HWND hWnd, uint msg, int wParam, [MarshalAs
        (UnmanagedType.U4)] int lParam);
 
        [DllImport("user32.dll", SetLastError=true)]
        public static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            UInt16 lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        WndClassEx wndClassEx; // IMPORTANT - otherwise the windowclass object will get GC'd
                               // and then the delegate we passed for the windowproc will be
                               // invalidated

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        private IntPtr GetWindowHandle()
        {
            //IntPtr hInstance = Marshal.GetHINSTANCE(this.GetType().Module);
            IntPtr hInstance = GetModuleHandle("LoaderLib2.dll");

            if (hInstance == new IntPtr(-1)) {
                throw new Win32Exception("Couldn't get modules instance");
            }

            //Hope this works.
            //Create our window class
            wndClassEx = new WndClassEx() {
                cbSize = (uint)Marshal.SizeOf(typeof(WndClassEx)),
                style = ClassStyles.GlobalClass,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hbrBackground = IntPtr.Zero,
                hCursor = IntPtr.Zero,
                hIcon = IntPtr.Zero,
                hIconSm = IntPtr.Zero,
                lpszClassName = "SharpShellMainClass",
                lpszMenuName = null,
                hInstance = hInstance,
                lpfnWndProc = this.WndProc
            };

            UInt16 atom = RegisterClassEx(ref wndClassEx);
            if (atom == 0)
                throw new Exception("Unable to register SharpShell Window Class");

            try {
                IntPtr hWnd = CreateWindowEx(
                    0,
                    atom,
                    "SharpShell Main Window",
                    0,
                    0, 0, 0, 0,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    hInstance,
                    IntPtr.Zero
                );

                if (hWnd == IntPtr.Zero) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                return hWnd;
            }
            catch {
                UnregisterClass("SharpShellMainClass", hInstance);
                throw;
            }
        }

        private uint CallBackMessage;
        private bool disposed;
        public HWND hWnd { get; private set; }

        public FullscreenDetector()
        {
            hWnd = IntPtr.Zero;
            FullscreenAppOpen = false;
            CreateNativeWindow();
        }

        void CreateNativeWindow()
        {

            hWnd = GetWindowHandle();
            if (IntPtr.Zero == hWnd) {
                throw new Exception("shit on a stick (window creation failed)");
            }
            APPBARDATA appBar = new APPBARDATA();
            appBar.cbSize = Marshal.SizeOf(appBar);
            appBar.hWnd = hWnd;

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

        public bool FullscreenAppOpen { get; set; }

        protected long WndProc(HWND hWnd, uint msg, int wParam, int lParam)
        {
            if (msg == this.CallBackMessage) {
                if (wParam == (int)ABNotify.ABN_FULLSCREENAPP) {
                    FullscreenAppOpen = (lParam != 0);

                    Console.WriteLine("A full-screen application is " +
                        (FullscreenAppOpen ? "opening" : "closing"));
                }
            }
            else if (msg == 0x0001) { //WM_CREATE
                return 0;
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if(!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if(disposing)
                {
                    // Dispose managed resources.
                    // TODO: add managed resources?
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                DestroyWindow(hWnd);
                CloseHandle(hWnd);
                hWnd = IntPtr.Zero;
                UnregisterClass("SharpShellMainClass", GetModuleHandle("LoaderLib2.dll"));
                // Note disposing has been done.
                disposed = true;

            }
        }

        // Use interop to call the method necessary
        // to clean up the unmanaged resource.
        [System.Runtime.InteropServices.DllImport("Kernel32")]
        private extern static Boolean CloseHandle(IntPtr handle);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyWindow(IntPtr hwnd);
        // Use C# destructor syntax for finalization code.
        // This destructor will run only if the Dispose method
        // does not get called.
        // It gives your base class the opportunity to finalize.
        // Do not provide destructors in types derived from this class.
        ~FullscreenDetector()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }
    }
}
