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

            var placeholder = peer.MasterDocumentPlaceHolder;
            Assert.IsNotNull(placeholder);
            Assert.AreEqual("MasterDocuments/7", placeholder.DocKey);
            Assert.AreEqual(7,placeholder.DocId);
            Assert.AreEqual("Bill", placeholder.Name);
        }




        [Test]
        public void Should_set_pointer_id_and_key_on_collection()
        {
            DetailDocument detail;
            var master = Helpers.BuildSimpleMasterAndDetail(out detail);

            Assert.AreEqual(1, master.DetailPlaceHolders.Count());
            Assert.AreEqual("DetailDocuments/25", master.DetailPlaceHolders.Single().DocKey);
        }




        [Test]
        public void Should_set_pointer_id_and_key_on_collection_reveresly()
        {
            DetailDocument detail;
            var master = Helpers.BuildSimpleMasterAndDetail(out detail);

            Assert.AreEqual(1, master.DetailPlaceHolders.Count());
            Assert.AreEqual("MasterDocuments/1", detail.MasterDocumentPointer.DocKey);
        }

    }

    public class Helpers
    {
        public static PeerDocument BuildSimpleMasterAndPeer(out MasterDocument master)
        {
            master = new MasterDocument { Id = 7, Name = "Bill"};
            var peer = new PeerDocument { Id = 100, Name="Frank", MasterDocumentPlaceHolder = new DocumentPlaceholder<MasterDocument>() };


            master.PeerDocumentPlaceHolder = DocumentPlaceholder<PeerDocument>.CreateFrom(peer,
                                                                                          a =>
                                                                                          a.MasterDocumentPlaceHolder,
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

            detail.MasterDocumentPointer = DocumentPlaceholder<MasterDocument>.CreateFrom(master,
                                                                                          a => a.DetailPlaceHolders,
                                                                                          detail);
            return master;
        }
    }
}