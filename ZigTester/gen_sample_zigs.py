import glob
import sys
import os

base_str = """command = "%s"
name = "%s"
description = "%s placeholder"
excluded_files = ["config.py"]
"""

def do_magic(src_path, dest_path = ''):
    root, dirs, files = os.walk(src_path).next()

    for dir in dirs:
        zigname = dir
        description = zigname
        base_path = os.path.join(root, dir)
        command = os.path.relpath(glob.glob(os.path.join(base_path, '*.exe'))[0], base_path)
        config_contents = base_str % (command, zigname, description)
        print 'writing config file to %s. Contents:' % os.path.join(base_path, "config.py")
        print config_contents
        with open(os.path.join(base_path, "config.py"), 'w') as cfg_file:
            cfg_file.write(config_contents)
        print 'now running zig maker, should output to %s.zig' % zigname
        command_str = "ziggen.py %s %s.zig" % (base_path, os.path.join(dest_path, dir))
        print 'command:', command_str
        os.system(command_str)

if __name__ == '__main__':
    do_magic(*sys.argv[1:])