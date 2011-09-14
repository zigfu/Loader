using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Services;
using System.Collections;


namespace LoaderLib
{
    public class CreateProcessEventArgs : EventArgs {
        public string Command;
        public string Path;
    }
    
    public class LoaderAPI : MarshalByRefObject
    {
        const int port = 32123;

        private event EventHandler<CreateProcessEventArgs> ServerCB;
        public LoaderAPI(EventHandler<CreateProcessEventArgs> callback)
        {
            ServerCB += callback;
        }

        public void LaunchProcess(string command, string path)
        {
            //Console.WriteLine("SERVER: cmd: {0}, wd: {1}", command, path);
            ServerCB(this, new CreateProcessEventArgs() { Path = path, Command = command });
        }

        public IntPtr ServerWindowHandle;

        public static LoaderAPI ConnectToServer()
        {
   			string url = string.Format(@"tcp://LocalHost:{0}/LoaderAPI", port);
			BinaryClientFormatterSinkProvider clientProvider = 
				new BinaryClientFormatterSinkProvider();

			BinaryServerFormatterSinkProvider serverProvider =
				new BinaryServerFormatterSinkProvider();
				
			serverProvider.TypeFilterLevel = 
				System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

			IDictionary props = new Hashtable();
			props["port"] = 0;
			props["name"] = System.Guid.NewGuid().ToString();
			props["typeFilterLevel"] = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
				
			TcpChannel chan = 
				new TcpChannel(props, clientProvider, serverProvider);

   			ChannelServices.RegisterChannel(chan, false);

            MarshalByRefObject obj = (MarshalByRefObject)RemotingServices.Connect(typeof(LoaderAPI), url);
            return obj as LoaderAPI;
        }

        public static LoaderAPI StartServer(IntPtr WindowHandle, EventHandler<CreateProcessEventArgs> callback)
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
			props["typeFilterLevel"] = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
				
			TcpChannel chan = 
				new TcpChannel(props, clientProvider, serverProvider);

			ChannelServices.RegisterChannel(chan, false);
	
			// Register the interface and end point.
			//
			// Each service has a unique endpoint and this endpoint is how a client
			// accesses a specific service on a remote server.
            LoaderAPI api = new LoaderAPI(callback);
            api.ServerWindowHandle = WindowHandle; //TODO: do this on HandleCreated event to ensure the right handle is passed
            RemotingServices.Marshal(api, "LoaderAPI");
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
