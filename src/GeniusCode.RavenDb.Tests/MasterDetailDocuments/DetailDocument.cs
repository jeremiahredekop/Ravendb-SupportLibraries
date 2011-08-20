using GeniusCode.RavenDb.Referential;

namespace GeniusCode.RavenDb.Tests.MasterDetailDocuments
{

    public class WhatDetailNeedsToKnowAboutMaster : IDocumentPointerData
    {
        #region Implementation of IDocumentPointerData

        public string Name { get; set; }

        #endregion
    }

    public class DetailDocument : IDocument
    {
        public DocumentPlaceholder<MasterDocument, WhatDetailNeedsToKnowAboutMaster> MasterDocumentPointer { get; set; }

        public string Name { get; set; }

        #region IDocument Members

        public int Id { get; set; }

        #endregion
    }
}