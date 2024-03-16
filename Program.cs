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

            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("API  http://localhost:4446/");
                Console.WriteLine("GET  api/documents");
                Console.WriteLine("POST api/upload/{filename}");
                Console.WriteLine("GET  api/delete/{filename}");
                Console.WriteLine("GET  api/load/{filename}");
                Console.WriteLine("GET  api/compress/{filename}");
                Console.WriteLine("POST api/compute");
                Console.ReadKey();
            }
        }
    }
}
