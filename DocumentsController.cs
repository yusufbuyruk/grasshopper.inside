using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using GrasshopperInside;

using GH_IO.Serialization;

namespace OwinSelfhostSample
{
    public class DocumentsController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetDocuments()
        {
            List<string> documents = new List<string>();

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "documents");

            if (Directory.Exists(folder))
                foreach (string file in Directory.GetFiles(folder))
                    if (Path.GetExtension(file) == ".gh" || Path.GetExtension(file) == ".ghx")
                        documents.Add(Path.GetFileName(file));
            
            Console.WriteLine("GET  | api/documents");
            return Json(documents);
        }

        [HttpPost]
        public IHttpActionResult Upload(string filename)
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "documents");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (Path.GetExtension(filename).ToLower() == ".ghx")
            {
                string document = Request.Content.ReadAsStringAsync().Result;

                using (StreamWriter writer = new StreamWriter(Path.Combine(folder, filename)))
                    writer.WriteLineAsync(document);

                Console.WriteLine($"POST | api/upload/{filename} | Ok");
                return Ok($"{filename} uploaded.");
            }
            else
            {
                Console.WriteLine($"POST | api/upload/{filename} | BadRequest");
                return BadRequest("Filename extension must be .ghx");
            }
        }

        [HttpGet]
        public IHttpActionResult Compress(string filename)
        {
            // Converts GHX to GH, uses Deflate algorithm

            if (Path.GetExtension(filename).ToLower() == ".ghx")
            {
                string compressedFilename = Path.GetFileNameWithoutExtension(filename) + ".gh";

                if (File.Exists(compressedFilename))
                {
                    Console.WriteLine($"GET  | api/compress/{filename} | BadRequest");
                    return BadRequest($"{compressedFilename} already exists.");
                }

                GH_Archive archive = new GH_Archive();
                archive.ReadFromFile(filename);
                bool result = archive.WriteToFile(compressedFilename, false, false);

                if (result)
                {
                    Console.WriteLine($"GET  | api/compress/{filename} | Ok");
                    return Ok($"{filename} compressed.");
                }

                else
                {
                    Console.WriteLine($"GET  | api/compress/{filename} | InternalServerError");
                    return InternalServerError();
                }
            }
            else
            {
                Console.WriteLine($"GET  | api/compress/{filename} | BadRequest");
                return BadRequest("Filename extension must be .ghx");
            }
        }

        [HttpGet]
        public IHttpActionResult Delete(string filename)
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "documents");

            string path = Path.Combine(folder, filename);

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);

                    Console.WriteLine($"GET  | api/delete/{filename} | Ok");
                    return Ok($"{filename} deleted");
                }
                else
                {
                    Console.WriteLine($"GET  | api/delete/{filename} | NotFound");
                    return NotFound();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET  | api/delete/{filename} | InternalServerError");
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult Load(string filename)
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "documents");

            if (File.Exists(Path.Combine(folder, filename)))
            {
                bool result = Document.GetDocument().Load(filename);

                if (result)
                {
                    Console.WriteLine($"GET  | api/load/{filename} | Ok");
                    return Json("document");
                }
                else
                {
                    Console.WriteLine($"GET  | api/load/{filename} | BadRequest");
                    return BadRequest("No cluster found.");
                }
            }
            else
            {
                Console.WriteLine($"GET  | api/load/{filename} | NotFound");
                return NotFound();
            }
        }
        

        [HttpPost]
        public IHttpActionResult Compute(int id)
        {
            try
            {
                string json = Request.Content.ReadAsStringAsync().Result;

                var clusters = Document.GetDocument().Clusters;

                if (clusters.Count > id)
                {
                    var cluster = clusters[id];
                    cluster.SetInputs(json);
                    cluster.ComputeOutputs();

                    Console.WriteLine("POST | api/compute | Ok");
                    return Json(cluster);
                }
                else
                {
                    Console.WriteLine("POST | api/compute | BadRequest");
                    return BadRequest("Index out of range.");
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("POST | api/compute | BadRequest");
                return InternalServerError(ex);
            }
        }
    }
}
