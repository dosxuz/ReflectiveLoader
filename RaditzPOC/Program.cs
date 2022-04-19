using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RaditzPOC
{
    class Program
    {
        static void Reflect(string filePath)
        {
            Assembly dotNetProgram = Assembly.LoadFile(filePath);
            Object[] parameters = new String[] { null };
            dotNetProgram.EntryPoint.Invoke(null, parameters);
        }
        static void Main(string[] args)
        {
            Reflect(@"C:\Users\ritab\Tools\HelloWorld\HelloWorld.exe");
        }
    }
}
