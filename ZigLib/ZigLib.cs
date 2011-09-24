using System;
using System.Collections.Generic;
using System.IO;
namespace ZigLib
{

	public class ZigLib
	{
        public const string ZigDBDefaultDirName = "Zigs";
        public const string ZigDefaultBaseURL = "http://django.zigfu.com/django";
        public static string GetDefaultZDBPath() {
            DirectoryInfo di = new DirectoryInfo(Utility.GetMainModuleDirectory());
            return Path.Combine(di.Parent.FullName, ZigDBDefaultDirName);
        }


        //TODO: something thread-safe?
        private static ZigDB _db;
        private static ZigDB db {
            get {
                if (_db == null) {
                    _db = new ZigDB(GetDefaultZDBPath(), ZigDefaultBaseURL);
                }
                return _db;
            }
        }

        public static void Init(string DBPath, string BaseURL)
        {
            if (null != _db) {
                throw new Exception("Double-init! :(");
            }
            _db = new ZigDB(DBPath, BaseURL);
        }
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

        // only call if you know that the remote zig is in fact installed (e.g. using IsZigInstalled())
        public static InstalledZig GetInstalledZig(RemoteZig zig)
        {
            return db.GetLocalZig(zig);
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

