using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Loader
{
    class Metadata
    {
        const string ZIG_VERSION = "zigVersion";
        const string VERSION = "version";
        const string COMMAND = "command";
        const string NAME = "name";

        public VersionString Version
        {
            get;
            private set;
        }

        public string Command
        {
            get;
            private set;
        }
        public string Name
        {
            get;
            private set;
        }

        const string EXPECTED_VERSION = "1";
        private string ZigVersion;
        public Metadata(string json)
        {
            //TODO: virtual C'tor accoridng to zigVersion
            Hashtable rootEntry = (Hashtable)JSON.JsonDecode(json);
            //TODO: if rootEntry is null then JSON is malformed. throw better exception.
            string zigVersion = (string)rootEntry[ZIG_VERSION];
            if (zigVersion != EXPECTED_VERSION) {
                throw new ArgumentException(string.Format("Bad zig metadata version: {0} (Expected: {1})", zigVersion, EXPECTED_VERSION));
            }
            ZigVersion = zigVersion;
            Version = new VersionString((string)rootEntry[VERSION]);
            Command = (string)rootEntry[COMMAND];
            Name = (string)rootEntry[NAME];
            //TODO: more metadata?
        }

        public static Metadata FromFile(string path)
        {
            return new Metadata(System.IO.File.ReadAllText(path));
        }

        public override string ToString()
        {
            return string.Format("<Metadata (v{0}). ZIG \"{1}\", version {2}, command: {3}>", ZigVersion, Name, Version, Command);
        }
    }
}
