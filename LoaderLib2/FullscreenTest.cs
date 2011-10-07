using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LoaderLib2
{

    class MyClass
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rc);


        public static bool isFullscreen(IntPtr hWnd)
        {
            IntPtr desktopHandle; //Window handle for the desktop
            IntPtr shellHandle; //Window handle for the shell
            //Get the handles for the desktop and shell now.
            desktopHandle = GetDesktopWindow();
            shellHandle = GetShellWindow();

            //Detect if the current app is running in full screen
            bool runningFullScreen = false;
            RECT appBounds;
            //IntPtr hWnd;

            //get the dimensions of the active window
            //hWnd = GetForegroundWindow();
            if (hWnd != null && !hWnd.Equals(IntPtr.Zero)) {
                //Check we haven't picked up the desktop or the shell
                if (!(hWnd.Equals(desktopHandle) || hWnd.Equals(shellHandle))) {
                    GetWindowRect(hWnd, out appBounds);
                    //determine if window is fullscreen
                    //screenBounds = Screen.FromHandle(hWnd).Bounds;
                    //if ((appBounds.Bottom - appBounds.Top) == screenBounds.Height && (appBounds.Right - appBounds.Left) == screenBounds.Width)
                    //{
                    //    runningFullScreen = true;
                    //}
                    RECT rc;
                    GetWindowRect(desktopHandle, out rc);
                    if ((appBounds.bottom == rc.bottom) && (appBounds.right == rc.right) && (appBounds.left == rc.left) && (appBounds.top == rc.top)) {
                        runningFullScreen = true;
                    }
                }
            }
            return runningFullScreen;
        }
    }
}
