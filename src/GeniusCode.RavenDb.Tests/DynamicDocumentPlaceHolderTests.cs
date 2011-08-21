using System.Dynamic;
using GeniusCode.RavenDb.Referential;
using GeniusCode.RavenDb.Tests.MasterDetailDocuments;
using NUnit.Framework;

namespace GeniusCode.RavenDb.Tests
{
    [TestFixture]
    public class DynamicDocumentPlaceHolderTests
    {
        [Test]
        public void ShouldBeAbleToSetValueOnDynamicData()
        {
            var placeholder = DocumentPlaceholder<PeerDocument>.CreatePlaceholder(100);

            placeholder.Data.BagData.Cool = "Cool";

            var placeholder2 = Helpers.SerializeCopyWithJSON(placeholder);
            Assert.AreEqual("Cool", placeholder2.Data.BagData.Cool);
        }


        [Test]
        public void JSONSupportsExpando()
        {
            dynamic e = new ExpandoObject();
            e.Hello = "World";
            var e2 = Helpers.SerializeDynamicCopyWithJSON<ExpandoObject>(e);
            Assert.AreEqual("World", e2.Hello);
        }

    }
}
