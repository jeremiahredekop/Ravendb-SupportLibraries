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
            dynamic data = new ExpandoObject();

            data.Cool = "Cool";
            //NOTE: we can't have dynamic, as JSON needs a concrete type for serialization!!
            DocumentPlaceholder<PeerDocument, ExpandoObject> placeholder = DocumentPlaceholder<PeerDocument, ExpandoObject>.CreatePlaceholder(100, data);

            var placeholder2 = Helpers.SerializeCopyWithJSON(placeholder);

            Assert.AreEqual("Cool", (placeholder2.Data as dynamic).Cool);



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
