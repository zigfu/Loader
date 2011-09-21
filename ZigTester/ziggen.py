import zipfile
import copy
import random
import os
from os import path

METADATA_FILE = ".metadata"
METADATA_CONFIG_FILE = "config.py"
METADATA_BASE = { "zig" :
                  { "description" : "Just another Lorem Ipsum placeholder",
                    "name" : "test random number: %d" % random.randint(1,9999),
                    "thumbnail_url" : ""
                  },
                  "version" : 0.0,
                }


allowed_zig_properties = ["name", "thumbnail_url", "description"]
COMMAND = 'command'

def json_from_python(some_dict):
    base = repr(some_dict)
    return base.replace("'", '"')

def read_config(base_path):
    config_str = open(path.join(base_path, METADATA_CONFIG_FILE)).read()
    locals = {}
    globals = {}
    exec config_str in globals, locals
    metadata_json = create_metadata(locals)
    filtered_files = []
    if locals.has_key('excluded_files'):
        filtered_files = locals['excluded_files']
    return metadata_json, filtered_files


def create_metadata(config_locals):
    if COMMAND not in config_locals:
        raise Exception("config file contains no command line (expected string variable named 'command')")
    metadata = copy.deepcopy(METADATA_BASE)
    metadata[COMMAND] = config_locals[COMMAND]
    for property in allowed_zig_properties:
        if property in config_locals:
            print 'found %s property, placing in zig: %s' % (property, config_locals[property])
            metadata["zig"][property] = config_locals[property]

    return json_from_python(metadata)
    

def create_zigfile(base_path, out_path):
    md, filterlist = read_config(base_path)
    print "The list of filtered files is:"
    for filtered in filterlist:
        print filtered
    # implicitly add existing metadata to filter list
    filterlist += [".metadata"]
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
    create_zigfile(sys.argv[1], sys.argv[2])

if __name__ == '__main__':
    main()