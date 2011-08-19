
namespace GeniusCode.RavenDb.Tests.MasterDetailDocuments
{

    public class PeerDocument :IDocument
    {
        #region Implementation of IDocument

        public int Id { get; set; }

        #endregion


        public string Name { get; set; }

        public DocumentPlaceholder<MasterDocument> MasterDocumentPlaceHolder { get; set; } 

    }

    public class MasterDocument : IDocument
    {
        public DocumentPlaceholder<PeerDocument> PeerDocumentPlaceHolder { get; set; } 

        public string Name { get; set; }
        public DocumentPlaceholderCollection<DetailDocument> DetailPlaceHolders { get; set; }

        #region IDocument Members

        public int Id { get; set; }

        #endregion
    }
}