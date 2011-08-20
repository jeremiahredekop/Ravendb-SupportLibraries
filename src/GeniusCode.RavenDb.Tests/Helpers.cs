using System;
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
                               MasterDocumentPlaceHolder = new DocumentPlaceholder<MasterDocument>()
                           };


            master.PeerDocumentPlaceHolder = DocumentPlaceholder<PeerDocument>.CreatePlaceholderAndReverse(peer.Id, peer.MasterDocumentPlaceHolder,
                                                                                                           master.Id);
            return peer;
        }

        public static MasterDocument BuildSimpleMasterAndDetail(out DetailDocument detail)
        {
            var master = new MasterDocument
                             {
                                 Id = 1,
                                 Name = "Chips",
                                 DetailPlaceHolders = new DocumentPlaceholderCollection<DetailDocument>()
                             };

            detail = new DetailDocument
                         {
                             Id = 25,
                             Name = "Cheetos"
                         };

            detail.MasterDocumentPointer =
                DocumentPlaceholder<MasterDocument>.CreatePlaceholderAndUpdateReverseCollection(master.Id, master.
                                                                                                    DetailPlaceHolders,
                                                                                                detail.Id);
            return master;
        }
    }
}