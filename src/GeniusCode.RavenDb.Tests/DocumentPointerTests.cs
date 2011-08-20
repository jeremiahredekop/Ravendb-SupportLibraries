using System;
using System.Linq;
using GeniusCode.RavenDb.Tests.MasterDetailDocuments;
using NUnit.Framework;

namespace GeniusCode.RavenDb.Tests
{
    [TestFixture]
    public class DocumentPlaceholderTests
    {

        [Test]
        public void Should_set_pointer_id_and_key_and_name()
        {
            MasterDocument master;
            var peer = Helpers.BuildSimpleMasterAndPeer(out master);

            AssertPointIdKeyAndName(master);
        }

        private static void AssertPointIdKeyAndName(MasterDocument master)
        {
            var placeHolder = master.PeerDocumentPlaceHolder;
            Assert.IsNotNull((placeHolder));
            Assert.AreEqual("PeerDocuments/100", placeHolder.DocKey);
            Assert.AreEqual(100, placeHolder.DocId);
            Assert.AreEqual("Frank", placeHolder.Name);
        }

        [Test]
        public void Should_set_pointer_id_and_key_and_name_reverseley()
        {
            MasterDocument master;
            var peer = Helpers.BuildSimpleMasterAndPeer(out master);

            AssertPointIdKeyAndNameReverse(peer);
        }

        private static void AssertPointIdKeyAndNameReverse(PeerDocument peer)
        {
            var placeholder = peer.MasterDocumentPlaceHolder;
            Assert.IsNotNull(placeholder);
            Assert.AreEqual("MasterDocuments/7", placeholder.DocKey);
            Assert.AreEqual(7, placeholder.DocId);
            Assert.AreEqual("Bill", placeholder.Name);
        }

        [Test]
        public void Should_set_pointer_id_and_key_on_collection()
        {
            DetailDocument detail;
            var master = Helpers.BuildSimpleMasterAndDetail(out detail);

            AssertPointIdAndKeyOnCollection(master);
        }

        private static void AssertPointIdAndKeyOnCollection(MasterDocument master)
        {
            var placeholder = master.DetailPlaceHolders.Items.Single();

            Assert.AreEqual(1, master.DetailPlaceHolders.Items.Count());
            Assert.AreEqual("DetailDocuments/25", placeholder.DocKey);
            Assert.AreEqual("Cheetos", placeholder.Name);
        }

        [Test]
        public void Should_set_pointer_id_and_key_on_collection_reveresly()
        {
            DetailDocument detail;
            var master = Helpers.BuildSimpleMasterAndDetail(out detail);

            AssertPointIdAndKeyOnCollectionReverse(detail);
        }

        private static void AssertPointIdAndKeyOnCollectionReverse(DetailDocument detail)
        {
            var placeholder = detail.MasterDocumentPointer;
            Assert.IsNotNull(placeholder);
            Assert.AreEqual("MasterDocuments/1", placeholder.DocKey);
            Assert.AreEqual("Chips", placeholder.Name);

        }

        [Test]
        public void Should_serialize_peer_pointers()
        {
            MasterDocument master;
            var peer = Helpers.BuildSimpleMasterAndPeer(out master);
            var master2 = Helpers.SerializeCopyWithJSON(master);
            AssertPointIdKeyAndName(master2);
        }

        [Test]
        public void Should_serialize_peer_pointers_reversely()
        {
            MasterDocument master;
            var peer = Helpers.BuildSimpleMasterAndPeer(out master);
            var peer2 = Helpers.SerializeCopyWithJSON(peer);

            AssertPointIdKeyAndNameReverse(peer2);
        }

        [Test]
        public void Should_serialize_collection_pointers()
        {
            DetailDocument detail;
            var master = Helpers.BuildSimpleMasterAndDetail(out detail);
            var master2 = Helpers.SerializeCopyWithJSON(master);
            AssertPointIdAndKeyOnCollection(master2);
        }

        [Test]
        public void Should_serialize_collection_pointers_reversely()
        {
            DetailDocument detail;
            var master = Helpers.BuildSimpleMasterAndDetail(out detail);
            var detail2 = Helpers.SerializeCopyWithJSON(detail);

            AssertPointIdAndKeyOnCollectionReverse(detail2);
        }

    }

    public class Helpers
    {
        public static T SerializeCopyWithJSON<T>(T input)
        {
            var s = input.SerializeToString();
            var fromString = s.DeserializeFromString<T>();
            return fromString;
        }

        public static PeerDocument BuildSimpleMasterAndPeer(out MasterDocument master)
        {
            master = new MasterDocument { Id = 7, Name = "Bill" };
            var peer = new PeerDocument { Id = 100, Name = "Frank", MasterDocumentPlaceHolder = new DocumentPlaceholder<MasterDocument>() };


            master.PeerDocumentPlaceHolder = DocumentPlaceholder<PeerDocument>.CreatePlaceholderAndReverse(peer,
                                                                                          peer.MasterDocumentPlaceHolder,
                                                                                          master);
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

            detail = new DetailDocument()
            {
                Id = 25,
                Name = "Cheetos"
            };

            detail.MasterDocumentPointer = DocumentPlaceholder<MasterDocument>.CreatePlaceholderAndUpdateReverseCollection(master,
                                                                                          master.DetailPlaceHolders,
                                                                                          detail);
            return master;
        }
    }
}