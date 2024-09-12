using Grasshopper.Plugin;
using Microsoft.Owin.Hosting;
using OwinSelfhostSample;
using Rhino;
using Rhino.Runtime.InProcess;
using RhinoInside;
using System;

namespace GrasshopperInside
{
    public class Program
    {
        static Program()
        {
            Resolver.RhinoSystemDirectory = @"C:\Program Files\Rhino 8\System";
            Resolver.Initialize();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("1/2 Starting RhinoCore...");
            using (var core = new RhinoCore())
            {
                RunHeadless();

                int port = 4446;

                if (args.Length > 0)
                    int.TryParse(args[0], out port);

                string baseAddress = $"http://localhost:{port}/";

                using (WebApp.Start<Startup>(url: baseAddress))  // bu satira bi WAIT gerekiyor
                {
                    Console.WriteLine("----------------------------");
                    Console.WriteLine($"API  {baseAddress}");
                    Console.WriteLine("----------------------------");
                    Console.WriteLine("GET  | api/documents");
                    Console.WriteLine("POST | api/upload/{filename}");
                    Console.WriteLine("GET  | api/delete/{filename}");
                    Console.WriteLine("GET  | api/load/{filename}");
                    Console.WriteLine("GET  | api/compress/{filename}");
                    Console.WriteLine("POST | api/compute/{id=0}");
                    Console.WriteLine("----------------------------");
                    Console.WriteLine("Press CTRL-Z to exit");
                    Console.WriteLine("----------------------------");

                    while (true)
                        if (Console.ReadLine() == null)
                            break;
                }
            }
        }

        static void RunHeadless()
        {
            Console.WriteLine("2/2 Loading Grasshopper...");
            var pluginObject = RhinoApp.GetPlugInObject("Grasshopper") as GH_RhinoScriptInterface;
            pluginObject.RunHeadless();
        }
    }
}
