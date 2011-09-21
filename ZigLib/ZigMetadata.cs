using System.Collections;
using System.IO;

namespace ZigLib
{
    internal class ZigProperties
    {
        public const string NAME = "name";
        public const string DESCRIPTION = "description";
        public const string ZIG_ID = "zigid";
        public const string ENTRY_ID = "entryid";
        public const string VERSION = "version";
        public const string DEVELOPER = "developer";
        public const string THUMBNAIL_URL = "thumbnail_url";
        public const string DOWNLOAD_URL = "dl_url";
        public const string RATING = "rating";
        public const string THUMBNAIL_PATH = "thumbnail_path";
    }

    public class SharedMetadata
    {
        public string Name { get; private set; }
        public string Developer { get; private set; }
        public string Description { get; private set; }
        public double ZigID { get; private set; }
        public double EntryID { get; private set; }
        public double Version { get; private set; }

        public static SharedMetadata FromFile(string path)
        {
            return new SharedMetadata(JSON.JsonDecode(File.ReadAllText(path)) as Hashtable);
        }

        public SharedMetadata(Hashtable properties)
        {
            Name = (string)properties[ZigProperties.NAME];
            Description = (string)properties[ZigProperties.DESCRIPTION];
            ZigID = (double)properties[ZigProperties.ZIG_ID];
            EntryID = (double)properties[ZigProperties.ENTRY_ID];
            Version = (double)properties[ZigProperties.VERSION];
            Developer = (string)properties[ZigProperties.DEVELOPER];
        }
    }
}