using System.Collections.Generic;
using System.IO;
using System.Web.Http;

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
            }

            return Ok();
        }

        [HttpGet]
        public IHttpActionResult Compress(string filename)
        {
            if (Path.GetExtension(filename).ToLower() == ".ghx")
            {
                // Converts GHX to GH, uses Deflate algorithm

            }

            return Ok();
        }

        [HttpGet]
        public IHttpActionResult Delete(string filename)
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "documents");

            if (File.Exists(Path.Combine(folder, filename)))
            {
                // Delete GH, return Ok();
            }

            return Ok("file not found");
        }

        [HttpGet]
        public IHttpActionResult Load(string filename)
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "documents");

            if (File.Exists(Path.Combine(folder, filename)))
            {
                // Load GH // return inputs outputs  // return Json(document)
            }

            // Load
            // Return inputs_outputs

            return Json("document");
        }
        

        [HttpPost]
        public IHttpActionResult Compute()
        {
            // Generate
            // Return inputs_outputs
            return Json("document");
        }
    }


    public class Document { }
}
