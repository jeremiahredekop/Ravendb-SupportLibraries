using System;
using System.Linq;
using System.Text;

namespace GeniusCode.RavenDb.Tests.MasterDetailDocuments
{
    public class DetailDocument : IDocument
    {
        public DocumentPlaceholder<MasterDocument> MasterDocumentPointer { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
