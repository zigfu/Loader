using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace ZigLib
{
    public class RemoteZig : IZig
    {
        

        public string RemoteURI { get; private set; }
        public double Rating { get; private set; }
        public string ThumbnailURI { get; private set; }
        public SharedMetadata Metadata { get; private set; }

        public RemoteZig(Hashtable DecodedJSON)
        {
            Metadata = new SharedMetadata(DecodedJSON);
            RemoteURI = (string)DecodedJSON[ZigProperties.DOWNLOAD_URL];
            ThumbnailURI = (string)DecodedJSON[ZigProperties.THUMBNAIL_URL];
        }

        public override string ToString()
        {
            return string.Format("RemoteZig: {0}, download URI: {1}", Metadata.Name, RemoteURI);
        }
    }
}
