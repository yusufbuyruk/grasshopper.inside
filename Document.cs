using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Special;
using GH_IO.Serialization;

namespace GrasshopperInside
{
    public class Document
    {
        // Singleton
        private static Document _document;

        public static Document Get()
        {
            if (_document == null)
                _document = new Document();
            
            return _document;
        }

        public string Filename(int id) => _documents[id];

        public List<Cluster> Clusters => _clusters;
        private readonly List<Cluster> _clusters;

        public List<string> Documents => _documents;
        private readonly List<string> _documents;

        private Document() 
        {
            _clusters = new List<Cluster>();
            _documents = new List<string>();
        }

        public bool Load(string filename)
        {
            GH_Document ghDocument = new GH_Document();
            GH_Archive archive = new GH_Archive();
            archive.ReadFromFile(filename);
            archive.ExtractObject(ghDocument, "Definition");

            foreach (var obj in ghDocument.Objects)
            {
                if (obj is GH_Cluster ghCluster)
                {
                    Cluster gi_cluster = new Cluster(ghCluster);
                    _clusters.Add(gi_cluster);
                }
            }

            return _clusters.Count > 0;
        }
    }
}