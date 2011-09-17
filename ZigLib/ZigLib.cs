using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ZigLib
{

	public class ZigLib
	{
        //TODO: ugh
        private static ZigDB db = new ZigDB("Zigs", "http://django.zigfu.com/django");


		public static IEnumerable<InstalledZig> EnumerateInstalledZigs()
		{
			return db.EnumerateInstalledZigs();
		}

        public static IEnumerable<RemoteZig> EnumerateRemoteZigs(string APIData)
		{
            return db.EnumerateRemoteZigs(APIData);
		}
		
		public static InstalledZig InstallZig(string localZigPath)
		{
            return db.InstallZig(localZigPath);
		}

        public static bool IsZigInstalled(IZig zig)
        {
            return db.IsInstalled(zig);
        }

		public static void LaunchZig(InstalledZig zigToLaunch)
		{
            zigToLaunch.Launch();
		}
		
		public static void RemoveZig(InstalledZig zigToRemove)
		{
            db.RemoveZig(zigToRemove);
		}

        public static string GetRemoteZigsQuery()
        {
            return db.GetRemoteZigsQuery();
        }

        public static string GetLocalZigQuery(InstalledZig zig)
        {
            return db.GetZigQuery(zig);
        }
        public static string GetLocalZigQuery(string zigId)
        {
            return db.GetZigQuery(zigId);
        }
    }
}

