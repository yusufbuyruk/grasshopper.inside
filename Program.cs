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
            Resolver.RhinoSystemDirectory = @"C:\Program Files\Rhino 7\System";
            Resolver.Initialize();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("1/2 Starting RhinoCore...");
            using (var core = new RhinoCore())
            {
                RunHeadless();


                string baseAddress = "http://localhost:4446/";

                if (args.Length > 0)
                    if (int.TryParse(args[0], out int port))
                        baseAddress = $"http://localhost:{port}";

                using (WebApp.Start<Startup>(url: baseAddress))
                {
                    Console.WriteLine("----------------------------");
                    Console.WriteLine("API  http://localhost:4446/");
                    Console.WriteLine("----------------------------");
                    Console.WriteLine("GET  | api/documents");
                    Console.WriteLine("POST | api/upload/{filename}");
                    Console.WriteLine("GET  | api/delete/{filename}");
                    Console.WriteLine("GET  | api/load/{filename}");
                    Console.WriteLine("GET  | api/compress/{filename}");
                    Console.WriteLine("POST | api/compute");
                    Console.WriteLine("----------------------------");
                    Console.WriteLine("Press CTRL-Z to exit");

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
