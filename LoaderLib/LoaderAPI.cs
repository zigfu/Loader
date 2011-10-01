using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Services;
using System.Collections;
using System.Threading;


namespace LoaderLib
{
    public class CreateProcessEventArgs : EventArgs {
        public string Command;
        public string Path;
        public int ProcessID;
    }
    
    // NOTE: I'm doing some very fishy things with named events here
    // I know that it's unsafe (it won't handle several concurrent clients for example)
    // but it'll be rewritten once that's actually required

    public class LoaderAPI : MarshalByRefObject
    {
        const int port = 32123;
        const string ServerEventName = "ZigFuServerExists";
        private EventWaitHandle ServerEvent;

        private event EventHandler<CreateProcessEventArgs> ServerCB;
        private event EventHandler ServerQuitCB;
        public LoaderAPI(EventHandler<CreateProcessEventArgs> callback, EventHandler quitCallback)
        {
            ServerCB += callback;
            ServerQuitCB += quitCallback;
            ServerEvent = new EventWaitHandle(false, EventResetMode.ManualReset, ServerEventName);
        }
        private void ReadyToServe()
        {
            ServerEvent.Set();
        }

        public void LaunchProcess(string command, string path, int pid)
        {
            //Console.WriteLine("SERVER: cmd: {0}, wd: {1}", command, path);
            ServerCB(this, new CreateProcessEventArgs() { Path = path, Command = command, ProcessID = pid });
        }

        public IntPtr ServerWindowHandle;

        public static LoaderAPI ConnectToServer()
        {
            if (!ServerExists()) {
                return null;
            }
            string url = GetUrlString();
            //string url = string.Format(@"ipc://LoaderProcess/LoaderAPI", port);
			BinaryClientFormatterSinkProvider clientProvider = 
				new BinaryClientFormatterSinkProvider();

			BinaryServerFormatterSinkProvider serverProvider =
				new BinaryServerFormatterSinkProvider();

            serverProvider.TypeFilterLevel =
                System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

			IDictionary props = new Hashtable();
			props["port"] = port;
            props["portName"] = "LoaderProcess";
			props["name"] = System.Guid.NewGuid().ToString();
            props["bindTo"] = "127.0.0.1";
            props["rejectRemoteRequests"] = true;
            props["typeFilterLevel"] = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
				
            //IpcClientChannel chan = 
            //    //new IpcChannel(props, clientProvider, serverProvider);
            //    new IpcClientChannel(props, clientProvider);
            TcpClientChannel chan =
                //new IpcChannel(props, clientProvider, serverProvider);
                new TcpClientChannel(props, clientProvider);


   			ChannelServices.RegisterChannel(chan, false);

            MarshalByRefObject obj = (MarshalByRefObject)RemotingServices.Connect(typeof(LoaderAPI), url);
            return obj as LoaderAPI;
        }

        private static string GetUrlString()
        {
            //TCP
            return string.Format(@"tcp://LocalHost:{0}/LoaderAPI", port);
            //IPC
            //return string.Format(@"ipc://LoaderProcess/LoaderAPI", port)
        }

        public static bool ServerExists()
        {
            try {
                // check if exists
                using (EventWaitHandle ServerExistsEvent = EventWaitHandle.OpenExisting(ServerEventName)) {
                    return ServerExistsEvent.WaitOne(0); // check if the event is set (it's set to true when the server is ready)
                }
            }
            catch (WaitHandleCannotBeOpenedException) {
                // event non-existent - open server
                return false;
            }
        }

        public static void LaunchServer()
        {
            Process.Start("Loader.exe");
        }

        public void KillServer()
        {
            ServerQuitCB(this, new EventArgs());
        }

        public static void ShutdownClient()
        {
            if (ServerExists()) {
                LoaderAPI api = ConnectToServer();
                api.KillServer();
            }
        }

        public static LoaderAPI StartServer(IntPtr WindowHandle, EventHandler<CreateProcessEventArgs> callback, EventHandler quitCallback)
        {
			
			BinaryClientFormatterSinkProvider clientProvider = null;

			BinaryServerFormatterSinkProvider serverProvider =
				new BinaryServerFormatterSinkProvider();

            serverProvider.TypeFilterLevel =
                System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

			IDictionary props = new Hashtable();

			/*
				* Client and server must use the SAME port
				* */
            props["port"] = port;
            props["portName"] = "LoaderProcess";
            props["name"] = System.Guid.NewGuid().ToString();
            props["typeFilterLevel"] = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
            props["bindTo"] = "127.0.0.1";
            props["rejectRemoteRequests"] = true;

            //IpcChannel chan = 
            //    new IpcChannel(props, clientProvider, serverProvider);
            //IpcServerChannel chan =
            //    new IpcServerChannel(props, serverProvider);
            TcpServerChannel chan =
                new TcpServerChannel(props, serverProvider);

			ChannelServices.RegisterChannel(chan, false);
	
			// Register the interface and end point.
			//
			// Each service has a unique endpoint and this endpoint is how a client
			// accesses a specific service on a remote server.
            LoaderAPI api = new LoaderAPI(callback, quitCallback);
            api.ServerWindowHandle = WindowHandle; //TODO: do this on HandleCreated event to ensure the right handle is passed
            RemotingServices.Marshal(api, "LoaderAPI");
            api.ReadyToServe();
            return api;
            //RemotingConfiguration.RegisterWellKnownServiceType(
            //        typeof(LoaderAPI), 
            //        "LoaderAPI", 
            //        WellKnownObjectMode.Singleton
            //    );
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

    }

}
