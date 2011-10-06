using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using HWND = System.IntPtr;
using System.ComponentModel;

namespace LoaderLib2
{
    class FullscreenDetector
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
            VerticalRedraw = 0x1
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

        private IntPtr GetWindowHandle()
        {
            IntPtr hInstance = Marshal.GetHINSTANCE(this.GetType().Module);

            if (hInstance == new IntPtr(-1)) {
                throw new Win32Exception("Couldn't get modules instance");
            }

            //Hope this works.
            //Create our window class
            WndClassEx wndClassEx = new WndClassEx() {
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
                    0x00000080,
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
        public HWND hWnd { get; private set; }

        public FullscreenDetector()
        {
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

        public bool FullscreenAppOpen { get; private set; }

        protected long WndProc(HWND hWnd, uint msg, int wParam, int lParam)
        {
            if (msg == this.CallBackMessage) {
                if (wParam == (int)ABNotify.ABN_FULLSCREENAPP) {
                    FullscreenAppOpen = (lParam != 0);

                    Console.WriteLine("A full-screen application is " +
                        (FullscreenAppOpen ? "opening" : "closing"));
                }
            }
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }
    }
}
