using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ZigLib
{

	public class ZigLib
	{
        //TODO: ugh
        private static ZigDB db = new ZigDB("Zigs");


		public static IEnumerable<InstalledZig> EnumerateInstalledZigs()
		{
			return db.EnumerateInstalledZigs();
		}

        public static IEnumerable<RemoteZig> EnumerateRemoteZigs(string APIData)
		{
            return db.EnumerateRemoteZigs(APIData);
		}
		
		public static void InstallZig(RemoteZig zigToInstall)
		{
            db.InstallZig(zigToInstall);
		}

		

		public static void LaunchZig(InstalledZig zigToLaunch)
		{
            zigToLaunch.Launch();
		}
		
		public static void RemoveZig(InstalledZig zigToRemove)
		{
			//TODO
		}
	}
}

