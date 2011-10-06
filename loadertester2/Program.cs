using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LoaderLib2;

namespace loadertester2
{
    class Program
    {
        static void Main(string[] args)
        {
            //UberLib ul = new UberLib();
            System.Threading.Thread.Sleep(1000);
            //ul.LaunchProcess(@"c:\windows\system32\notepad.exe", "");
            //API.LaunchProcess(@"c:\windows\system32\notepad.exe", "");
            Console.WriteLine("launched, hit enter to quit");
            Console.ReadLine();
        }
    }
}
