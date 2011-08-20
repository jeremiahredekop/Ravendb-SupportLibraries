namespace GeniusCode.RavenDb.Tests.MasterDetailDocuments
{
    public class DetailDocument : IDocument
    {
        public DocumentPlaceholder<MasterDocument> MasterDocumentPointer { get; set; }

        public string Name { get; set; }

        #region IDocument Members

        public int Id { get; set; }

        #endregion
    }
}