import zipfile
import copy
import random
import os
from os import path

METADATA_FILE = ".metadata"

COMMAND = 'command'
METADATA_VERSION = 1.0

def autodetect_os():
    if os.name == "nt":
        return "Win"
    elif os.name == "posix":
        # assume mac because we don't support linux
        return "Mac"
    else:
        raise Exception("Unhandled OS")

# list of (property-name, conversion function, default value)
zig_properties = [('command', str),
                          ("thumbnail_path", str),
                          ("name", str, "test random number: %d" % random.randint(1,9999)),
                          ("description", str, "Just another Lorem Ipsum placeholder"),
                          ("os", str, autodetect_os()),
                          ("developer", str, "Scopeware"),
                          ("version", float, 0.0),
                          ("entryid", str, ""),
                          ("zigid", str, ""),
                          ("metadata_version", float, METADATA_VERSION),
                          ]

def json_from_python(some_dict):
    base = repr(some_dict)
    return base.replace("'", '"')



def create_metadata(*params):
    
    metadata = dict( (i[0], i[1](value) if value else i[2]) for i, value in map(None, zig_properties, params))
    return json_from_python(metadata)
    

def create_zigfile(base_path, out_path, *parameters):
    md = create_metadata(*parameters)
    
    # implicitly add existing metadata to filter list
    filterlist = [".metadata"]
    with zipfile.ZipFile(out_path,"w", zipfile.ZIP_DEFLATED) as out_zig:
##        if not path.exists(path.join(base_path, METADATA_FILE)):
##            print "no metadata file, trying to use %s to generate metadata" % METADATA_CONFIG_FILE
##            out_zig.writestr(METADATA_FILE, md)
##        else:
##            print "using existing metadata"
        out_zig.writestr(METADATA_FILE, md) #TODO: use something like the above if statement instead

        for root, dirs, files in os.walk(base_path):
            for filename in files:
                filepath = path.join(root, filename)
                filepath_in_zip = path.relpath(filepath, base_path)
                if filepath_in_zip in filterlist:
                    print "file %s in filter list - skipping!" % filepath_in_zip
                    continue
                print "Adding %s to zip as %s" % (filepath, filepath_in_zip)
                out_zig.write(filepath, filepath_in_zip)
    print 'done!'

def main():
    import sys
    
    create_zigfile(*sys.argv[1:])

if __name__ == '__main__':
    main()