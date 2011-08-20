using GeniusCode.RavenDb.Referential;

namespace GeniusCode.RavenDb.Tests.MasterDetailDocuments
{
    public class PeerDocument : IDocument
    {
        #region Implementation of IDocument

        public int Id { get; set; }

        #endregion

        public string Name { get; set; }

        public DocumentPlaceholder<MasterDocument, WhatPeerNeedsToKnowAboutMaster> MasterDocumentPlaceHolder { get; set; }
    }

    public class WhatPeerNeedsToKnowAboutMaster : IDocumentPointerData
    {
        #region Implementation of IDocumentPointerData

        public string Name { get; set; }

        #endregion
    }

    public class MasterDocument : IDocument
    {
        public DocumentPlaceholder<PeerDocument, WhatMasterNeedsToKnowAboutPeer> PeerDocumentPlaceHolder { get; set; }

        public string Name { get; set; }
        public DocumentPlaceholderCollection<DetailDocument, WhatMasterNeedsToKnowAboutDetail> DetailPlaceHolders { get; set; }

        #region IDocument Members

        public int Id { get; set; }

        #endregion
    }

    public class WhatMasterNeedsToKnowAboutPeer : IDocumentPointerData
    {
        #region Implementation of IDocumentPointerData

        public string Name { get; set; }

        #endregion
    }
    public class WhatMasterNeedsToKnowAboutDetail : IDocumentPointerData
    {
        #region Implementation of IDocumentPointerData

        public string Name { get; set; }

        #endregion
    }


}