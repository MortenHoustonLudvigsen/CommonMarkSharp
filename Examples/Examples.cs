using CommonMarkSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    public static partial class Examples
    {
        /// <summary>
        /// Reading from a file, and rendering HTML to another file 
        /// </summary>
        public static void ReadFromFileRenderToFile()
        {
            var cm = new CommonMark();

            using (var reader = File.OpenText("Example.md"))
            using (var writer = File.CreateText("ReadFromFileRenderToFile.html"))
            {
                cm.RenderAsHtml(reader, writer);
            }
        }

        /// <summary>
        /// Reading from a file, and rendering HTML to a string
        /// </summary>
        public static void ReadFromFileRenderToString()
        {
            var cm = new CommonMark();

            using (var reader = File.OpenText("Example.md"))
            {
                var html = cm.RenderAsHtml(reader);
            }
        }

    }
}