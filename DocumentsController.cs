using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Http;
using GrasshopperInside;

using GH_IO.Serialization;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace OwinSelfhostSample
{
    public class DocumentsController : ApiController
    {

        [HttpGet]
        public IHttpActionResult GetDocuments()
        {
            var documents = Document.Get().Documents;
            documents.Clear();

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "documents");

            if (Directory.Exists(folder))
                foreach (string file in Directory.GetFiles(folder))
                    if (Path.GetExtension(file) == ".gh" || Path.GetExtension(file) == ".ghx")
                        documents.Add(Path.GetFileName(file));
            
            Console.WriteLine("GET  | api/documents");

            //var data = new
            //{
            //    documents = documents
            //};

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
        public IHttpActionResult Compress(int id)
        {
            // Converts GHX to GH, uses Deflate algorithm

            var document = Document.Get();

            if (document.Documents.Count >= id)
                return BadRequest("Index out of range.");

            string filename = document.Filename(id);
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "documents");
            string path = Path.Combine(folder, filename);

            if (!File.Exists(path))
            {
                Console.WriteLine($"GET  | api/compress/{id} - {filename} | NotFound");
                return BadRequest($"{filename} not found.");
            }

            if (Path.GetExtension(filename).ToLower() == ".ghx")
            {
                string compressedFilename = Path.GetFileNameWithoutExtension(filename) + ".gh";

                if (File.Exists(compressedFilename))
                {
                    Console.WriteLine($"GET  | api/compress/{id} - {filename} | BadRequest");
                    return BadRequest($"{compressedFilename} already exists.");
                }

                GH_Archive archive = new GH_Archive();
                archive.ReadFromFile(filename);
                bool result = archive.WriteToFile(compressedFilename, false, false);

                if (result)
                {
                    Console.WriteLine($"GET  | api/compress/{id} - {filename} | Ok");
                    return Ok($"{filename} compressed.");
                }

                else
                {
                    Console.WriteLine($"GET  | api/compress/{id} - {filename} | InternalServerError");
                    return InternalServerError();
                }
            }
            else
            {
                Console.WriteLine($"GET  | api/compress/{id} - {filename} | BadRequest");
                return BadRequest("Filename extension must be .ghx");
            }
        }

        [HttpGet]
        public IHttpActionResult Delete(int id)
        {
            var document = Document.Get();

            if (document.Documents.Count >= id)
                return BadRequest("Index out of range.");

            string filename = document.Filename(id);
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "documents");
            string path = Path.Combine(folder, filename);

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);

                    Console.WriteLine($"GET  | api/delete/{id} - {filename} | Ok");
                    return Ok($"{filename} deleted");
                }
                else
                {
                    Console.WriteLine($"GET  | api/delete/{id} - {filename} | NotFound");
                    return NotFound();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GET  | api/delete/{id} - {filename} | InternalServerError");
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult Load(int id)
        {
            var document = Document.Get();

            if (id > document.Documents.Count)
                return BadRequest("Index out of range.");

            string filename = document.Filename(id);
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "documents");
            string path = Path.Combine(folder, filename);

            if (File.Exists(path))
            {
                bool result = document.Load(path); // (clusters.Count > 0)

                if (result)
                {
                    Console.WriteLine($"GET  | api/load/{id} - {filename} | Ok");
                    return Json(document.Clusters.Count);
                }
                else
                {
                    Console.WriteLine($"GET  | api/load/{id} - {filename} | BadRequest");
                    return BadRequest("No cluster found.");
                }
            }
            else
            {
                Console.WriteLine($"GET  | api/load/{id} - {filename} | NotFound");
                return NotFound();
            }
        }

        [HttpGet]
        public IHttpActionResult GetCluster(int id = 0)
        {
            var document = Document.Get();

            if (id >= document.Clusters.Count)
                return BadRequest("Index out of range.");



            var cluster = document.Clusters[id];
            return Json(cluster.Inputs);
        }

        [HttpPost]
        public IHttpActionResult Compute(int id = 0)
        {
            var document = Document.Get();

            if (document.Clusters.Count >= id)
            {
                Console.WriteLine($"POST | api/compute/{id} | BadRequest | Index out of range");
                return BadRequest("Index out of range.");
            }

            try
            {
                string json = Request.Content.ReadAsStringAsync().Result;

                var clusters = Document.Get().Clusters;

                var cluster = clusters[id];

                cluster.SetInputs(json);
                cluster.ComputeOutputs();

                Console.WriteLine($"POST | api/compute/{id} | Ok");
                return Json(cluster);

            }

            catch (Exception ex)
            {
                Console.WriteLine($"POST | api/compute/{id} | BadRequest");
                return InternalServerError(ex);
            }
        }
    }
}
