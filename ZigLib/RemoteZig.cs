using System;
using System.Collections.Generic;
using System.Linq;
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

        public RemoteZig(string RawJSON)
        {
            Hashtable props = JSON.JsonDecode(RawJSON) as Hashtable;
            RemoteURI = (string)props["dl_url"];
            Metadata = new ZigMetadata(props);
        }

    }
}
