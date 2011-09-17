﻿using System.Collections;
using System.IO;

namespace ZigLib
{
    public class ZigMetadata
    {
        public string Name { get; private set; }
        public string Developer { get; private set; }
        public string Description { get; private set; }
        public string ThumbnailURI { get; private set; }
        public string ID { get; private set; }

        public static ZigMetadata FromFile(string path)
        {
            return new ZigMetadata(JSON.JsonDecode(File.ReadAllText(path)) as Hashtable);
        }

        const string NAME = "name";
        const string THUMBNAIL = "thumbnail_url";
        const string DESCRIPTION = "description";
        const string ZIG_ID = "zigid";

        public ZigMetadata(Hashtable properties)
        {
            Name = (string)properties[NAME];
            Description = (string)properties[DESCRIPTION];
            ThumbnailURI = (string)properties[THUMBNAIL];
            ID = (string)properties[ZIG_ID];

            Developer = "DevCo"; //TODO: real data
        }
    }
}