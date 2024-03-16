using Microsoft.Owin.Hosting;
using OwinSelfhostSample;
using System;

namespace GrasshopperInside
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:4446/";

            if (args.Length > 0)
                if (int.TryParse(args[0], out int port))
                    baseAddress = $"http://localhost:{port}";

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("API  http://localhost:4446/");
                Console.WriteLine("GET  api/documents");
                Console.WriteLine("POST api/upload/{filename}");
                Console.WriteLine("GET  api/delete/{filename}");
                Console.WriteLine("GET  api/load/{filename}");
                Console.WriteLine("GET  api/compress/{filename}");
                Console.WriteLine("POST api/compute");
                Console.WriteLine("");
                Console.WriteLine("Press CTRL-Z to exit");

                while (true)
                    if (Console.ReadLine() == null)
                        break;
            }
        }
    }
}
