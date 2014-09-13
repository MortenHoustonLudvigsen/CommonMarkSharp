using CommonMarkSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            TestIt("ReadFromFileRenderToFile", Examples.ReadFromFileRenderToFile);
            TestIt("ReadFromFileRenderToString", Examples.ReadFromFileRenderToString);
            //Console.ReadLine();
        }
    
        private static void TestIt(string example, Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Restart();
            action();
            stopwatch.Stop();
            Console.WriteLine("{0} - elapsed: {1} ms", example, stopwatch.ElapsedMilliseconds);
        }
    }
}
