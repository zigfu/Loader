using System;
using System.Collections.Generic;

namespace ZigLib
{
	public class ZigMetadata
	{
		public string name;
		public string developer;
		public string description;
		public string thumbnail_uri;
	}
	
	public class InstalledZig
	{
		ZigMetadata metadata;
		string path;
	}
	
	public class RemoteZig
	{
		ZigMetadata metadata;
		string zig_uri;
	}
	
	public class ZigLib
	{
		public static List<InstalledZig> EnumerateInstalledZigs()
		{
			return new List<InstalledZig>();
		}
		
		public static List<RemoteZig> EnumerateRemoteZigs()
		{
			return new List<RemoteZig>();
		}
		
		public static void InstallZig(RemoteZig zigToInstall)
		{
			
		}
		
		public static void LaunchZig(InstalledZig zigToLaunch)
		{
			
		}
		
		public static void RemoveZig(InstalledZig zigToRemove)
		{
			
		}
	}
}

