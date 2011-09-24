import glob
import sys
import os
import subprocess

def generate_config(root, zig_dir, os_str):
        zigname = zig_dir
        description = zigname
        base_path = os.path.join(root, zig_dir)
        # find random exe
        command = os.path.relpath(glob.glob(os.path.join(base_path, '*.exe'))[0], base_path)
        # use default name or first image in dir if exists
        thumbnail = "thumbnail.png"
        images = sum([glob.glob(os.path.join(base_path, pattern)) for pattern in ["*.png", "*.jpg"]], [])
        if images:
            thumbnail = os.path.relpath(images[0], base_path)
        config = [command, thumbnail, zigname, description]
        if os_str is not None:
            config.append(os_str)
        return config

def do_magic(src_path, dest_path = '', os_str = None):
    root, dirs, files = os.walk(src_path).next()
    if os_str is not None:
        print 'got operating system override in command-line: ' + os_str
    for dir in dirs:
        base_path = os.path.join(root, dir)
        zigname = os.path.join(dest_path, dir) + ".zig"
        print 'now running zig maker, should output to %s' % zigname
        commands = ["ziggen.py", base_path, zigname]
        commands.extend(generate_config(root, dir, os_str))
        print 'command: ', " ".join(commands)
        subprocess.Popen(commands, shell=True).communicate()

if __name__ == '__main__':
    do_magic(*sys.argv[1:])
    