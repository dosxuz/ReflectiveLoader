using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Net;
using System.Threading;

namespace CellAppDomain
{
    class Program
    {
        public class Worker : MarshalByRefObject
        {
            public void Reflect(string filePath)
            {
                Assembly dotNetProgram = Assembly.LoadFile(filePath);
                Object[] parameters = new String[] { null };
                dotNetProgram.EntryPoint.Invoke(null, parameters);
            }
            public void ReflectFromWeb(string url, int retrycount=0, int timeoutTimer=0)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                WebClient client = new WebClient();
                byte[] programBytes = null;

                while(retrycount >= 0 && programBytes == null)
                {
                    try
                    {
                        programBytes = client.DownloadData(url);
                    }
                    catch (WebException e)
                    {
                        Console.WriteLine("Assembly not found yet. Sleeping for {0} seconds and retrying another {1} time(s)...", timeoutTimer, retrycount);
                        retrycount--;
                        Thread.Sleep(timeoutTimer * 1000);
                    }
                }

                if (programBytes == null)
                {
                    Console.WriteLine("Assembly was not found, exitting now....");
                    Environment.Exit(-1);
                }

                Assembly dotNetProgram = Assembly.Load(programBytes);
                Object[] parameters = new String[] { null };
                dotNetProgram.EntryPoint.Invoke(null, parameters);
            }
        }
        static void Main(string[] args)
        {
            AppDomain namek = AppDomain.CreateDomain("Namek");
            Console.WriteLine("AppDomain Namek created");
            Console.ReadKey();
            Worker localworker = (Worker)namek.CreateInstanceAndUnwrap(typeof(Worker).Assembly.FullName, new Worker().GetType().FullName);
            localworker.Reflect(@"C:\Users\ritab\Tools\HelloWorld\HelloWorld.exe");
            Console.ReadKey();
            Console.WriteLine("Unloaded Namek");
            AppDomain.Unload(namek);

            //Creating new appdomain to download from web server
            AppDomain hagu = AppDomain.CreateDomain("Hagu");
            Console.WriteLine("AppDomain Hagu Created");
            Console.ReadKey();
            Worker remoteworker = (Worker)hagu.CreateInstanceAndUnwrap(typeof(Worker).Assembly.FullName, new Worker().GetType().FullName);
            remoteworker.ReflectFromWeb("http://192.168.43.118/HelloWorldFromWeb.exe");
            Console.WriteLine("Unloaded Hagu");
            AppDomain.Unload(hagu);
            Console.ReadKey();
        }
    }
}
