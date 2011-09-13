using UnityEngine;
using System.Collections;
using LoaderLib;
using System.Runtime.InteropServices;
using System;

public class LoaderCreateProcess : MonoBehaviour {

    public string CommandToRun = @"c:\windows\system32\notepad.exe";
	// Use this for initialization
	void Start () {
	
	}

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool SetForegroundWindow(IntPtr hWnd);
	// Update is called once per frame
	void OnGUI () {
        if (Event.current.type == EventType.KeyDown) {
            if (Event.current.keyCode == KeyCode.Space) {
                var item = LoaderLib.LoaderAPI.ConnectToServer(12345);
                SetForegroundWindow(item.ServerWindowHandle);
                item.LaunchProcess(CommandToRun, "test");
                Event.current.Use();
            }
            if (Event.current.keyCode == KeyCode.Escape) {
                Application.Quit();
            }
        }
	}
}
