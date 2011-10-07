using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
namespace LoaderLib2
{
    /// <summary>
    /// Gets all the handles for top level windows, and then determines the pids that have windows
    /// </summary>
    public class WindowFinder
    {
        #region Win32Functions
        [DllImport("user32.dll")]
        private static extern int EnumWindows(EnumWindowsProc ewp, int lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        public delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);
        #endregion

        #region Data
        private List<IntPtr> windowHandles;
        private List<UInt32> pidsWithWindows;
        private System.Threading.Mutex accessMutex;
        private EnumWindowsProc proc;
        #endregion

        #region Methods
        public WindowFinder()
        {
            accessMutex = new System.Threading.Mutex();
            windowHandles = new List<IntPtr>();
            pidsWithWindows = new List<UInt32>();
            proc = new EnumWindowsProc(EnumWindowsCallback);
        }
        public void Update()
        {
            accessMutex.WaitOne();
            windowHandles.Clear();
            pidsWithWindows.Clear();
            accessMutex.ReleaseMutex();
            EnumWindows(proc, 0);
        }
        public UInt32 GetPidOfWindow(IntPtr hwnd)
        {
            uint temp;
            GetWindowThreadProcessId(hwnd, out temp);
            return temp;
        }
        private bool EnumWindowsCallback(IntPtr hwnd, int lparam)
        {
            accessMutex.WaitOne();
            //*******************Perhaps some code right here that determines if it is the main window*************
            windowHandles.Add(hwnd);
            pidsWithWindows.Add(GetPidOfWindow(hwnd));
            accessMutex.ReleaseMutex();
            return true;
        }
        public Boolean DoesPidHaveMainWindow(Int32 pid)
        {
            return pidsWithWindows.Contains((UInt32)pid);
        }
        public IntPtr MainWindowOfPid(Int32 pid)
        {
            accessMutex.WaitOne();
            if (!DoesPidHaveMainWindow(pid)) return IntPtr.Zero;
            Int32 index = pidsWithWindows.IndexOf((UInt32)pid);
            IntPtr handle = windowHandles[index];
            accessMutex.ReleaseMutex();
            return handle;
        }
        #endregion
    }
}
