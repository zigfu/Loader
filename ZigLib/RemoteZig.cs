using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ZigLib
{
    public class RemoteZig
    {
        public ZigMetadata Metadata { get; private set; }
        public string RemoteURI { get; private set; }

        public RemoteZig(string RemoteURI, ZigMetadata Metadata)
        {
            this.RemoteURI = RemoteURI;
            this.Metadata = Metadata;
        }

        public RemoteZig(Hashtable DecodedJSON)
        {
            RemoteURI = (string)DecodedJSON["dl_url"];
            Metadata = new ZigMetadata(DecodedJSON["zig"] as Hashtable);
        }

        public override string ToString()
        {
            return string.Format("RemoteZig: {0}, download URI: {1}", Metadata.Name, RemoteURI);
        }
    }
}
