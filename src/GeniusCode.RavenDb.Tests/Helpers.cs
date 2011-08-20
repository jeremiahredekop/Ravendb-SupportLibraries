using System;
using GeniusCode.RavenDb.Referential;
using GeniusCode.RavenDb.Tests.MasterDetailDocuments;

namespace GeniusCode.RavenDb.Tests
{
    public class Helpers
    {
        public static T SerializeCopyWithJSON<T>(T input)
        {
            string s = input.SerializeToString();
            var fromString = s.DeserializeFromString<T>();
            return fromString;
        }

        public static PeerDocument BuildSimpleMasterAndPeer(out MasterDocument master)
        {
            master = new MasterDocument { Id = 7, Name = "Bill" };
            var peer = new PeerDocument
                           {
                               Id = 100,
                               Name = "Frank",
                               MasterDocumentPlaceHolder = new DocumentPlaceholder<MasterDocument, WhatPeerNeedsToKnowAboutMaster>()
                           };

            var peerData = new WhatMasterNeedsToKnowAboutPeer { Name = peer.Name };

            var masterData = new WhatPeerNeedsToKnowAboutMaster { Name = master.Name };


            master.PeerDocumentPlaceHolder = DocumentPlaceholder<PeerDocument, WhatMasterNeedsToKnowAboutPeer>
                .CreatePlaceholderAndReverse(peer.Id, peerData, peer.MasterDocumentPlaceHolder, master.Id, masterData);
            return peer;
        }

        public static MasterDocument BuildSimpleMasterAndDetail(out DetailDocument detail)
        {
            var master = new MasterDocument
                             {
                                 Id = 1,
                                 Name = "Chips",
                                 DetailPlaceHolders = new DocumentPlaceholderCollection<DetailDocument, WhatMasterNeedsToKnowAboutDetail>()
                             };

            var masterData = new WhatDetailNeedsToKnowAboutMaster { Name = master.Name };

            detail = new DetailDocument
                         {
                             Id = 25,
                             Name = "Cheetos"
                         };
            var detailData = new WhatMasterNeedsToKnowAboutDetail { Name = detail.Name };


            detail.MasterDocumentPointer =
                DocumentPlaceholder<MasterDocument, WhatDetailNeedsToKnowAboutMaster>
                .CreatePlaceholderAndUpdateReverseCollection(master.Id, masterData, master.DetailPlaceHolders, detail.Id, detailData);
            return master;
        }
    }
}