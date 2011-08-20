using System.Linq;
using GeniusCode.RavenDb.Tests.MasterDetailDocuments;
using NUnit.Framework;

namespace GeniusCode.RavenDb.Tests
{
    [TestFixture]
    public class DocumentPlaceholderTests
    {
        #region private helpers

        private static void AssertPointIdKeyAndName(MasterDocument master)
        {
            var placeHolder = master.PeerDocumentPlaceHolder;
            Assert.IsNotNull((placeHolder));
            Assert.AreEqual("PeerDocuments/100", placeHolder.DocKey);
            Assert.AreEqual(100, placeHolder.DocId);
            //Assert.AreEqual("Frank", placeHolder.Name);
        }

        private static void AssertPointIdKeyAndNameReverse(PeerDocument peer)
        {
            var placeholder = peer.MasterDocumentPlaceHolder;
            Assert.IsNotNull(placeholder);
            Assert.AreEqual("MasterDocuments/7", placeholder.DocKey);
            Assert.AreEqual(7, placeholder.DocId);
            //Assert.AreEqual("Bill", placeholder.Name);
        }

        private static void AssertPointIdAndKeyOnCollection(MasterDocument master)
        {
            var placeholder = master.DetailPlaceHolders.Items.Single();

            Assert.AreEqual(1, master.DetailPlaceHolders.Items.Count());
            Assert.AreEqual("DetailDocuments/25", placeholder.DocKey);
            //Assert.AreEqual("Cheetos", placeholder.Name);
        }

        private static void AssertPointIdAndKeyOnCollectionReverse(DetailDocument detail)
        {
            var placeholder = detail.MasterDocumentPointer;
            Assert.IsNotNull(placeholder);
            Assert.AreEqual("MasterDocuments/1", placeholder.DocKey);
            //Assert.AreEqual("Chips", placeholder.Name);
        }

        #endregion

        [Test]
        public void Should_serialize_collection_pointers()
        {
            DetailDocument detail;
            MasterDocument master = Helpers.BuildSimpleMasterAndDetail(out detail);
            MasterDocument master2 = Helpers.SerializeCopyWithJSON(master);
            AssertPointIdAndKeyOnCollection(master2);
        }

        [Test]
        public void Should_serialize_collection_pointers_reversely()
        {
            DetailDocument detail;
            Helpers.BuildSimpleMasterAndDetail(out detail);
            DetailDocument detail2 = Helpers.SerializeCopyWithJSON(detail);

            AssertPointIdAndKeyOnCollectionReverse(detail2);
        }

        [Test]
        public void Should_serialize_peer_pointers()
        {
            MasterDocument master;
            Helpers.BuildSimpleMasterAndPeer(out master);
            MasterDocument master2 = Helpers.SerializeCopyWithJSON(master);
            AssertPointIdKeyAndName(master2);
        }

        [Test]
        public void Should_serialize_peer_pointers_reversely()
        {
            MasterDocument master;
            PeerDocument peer = Helpers.BuildSimpleMasterAndPeer(out master);
            PeerDocument peer2 = Helpers.SerializeCopyWithJSON(peer);

            AssertPointIdKeyAndNameReverse(peer2);
        }

        [Test]
        public void Should_set_pointer_id_and_key_and_name()
        {
            MasterDocument master;
            Helpers.BuildSimpleMasterAndPeer(out master);
            AssertPointIdKeyAndName(master);
        }

        [Test]
        public void Should_set_pointer_id_and_key_and_name_reverseley()
        {
            MasterDocument master;
            PeerDocument peer = Helpers.BuildSimpleMasterAndPeer(out master);

            AssertPointIdKeyAndNameReverse(peer);
        }

        [Test]
        public void Should_set_pointer_id_and_key_on_collection()
        {
            DetailDocument detail;
            MasterDocument master = Helpers.BuildSimpleMasterAndDetail(out detail);

            AssertPointIdAndKeyOnCollection(master);
        }

        [Test]
        public void Should_set_pointer_id_and_key_on_collection_reveresly()
        {
            DetailDocument detail;
            Helpers.BuildSimpleMasterAndDetail(out detail);

            AssertPointIdAndKeyOnCollectionReverse(detail);
        }
    }
}