using System;
using GeniusCode.RavenDb.Tests.MasterDetailDocuments;
using NUnit.Framework;

namespace GeniusCode.RavenDb.Tests
{
    [TestFixture]
    public class JSonSerializeTests
    {
        [Test]
        public void ShouldDeserializeCollection()
        {
            const string myString = "{\"Items\":[{\"DocKey\":\"DetailDocuments/25\",\"Name\":\"Cheetos\"}]}";
            var obj = myString.DeserializeFromString<DocumentPlaceholderCollection<DetailDocument>>();
            Assert.IsNotNull(obj);
        }

        [Test]
        public void ShouldSerialize()
        {
            var master = new MasterDocument { Name = "Frank" };
            string masterString = master.SerializeToString();
            var master2 = masterString.DeserializeFromString<MasterDocument>();
            Assert.AreEqual("Frank", master2.Name);
        }

        [Test]
        public void ShouldSerializeCollection()
        {
            DetailDocument detail;
            MasterDocument master = Helpers.BuildSimpleMasterAndDetail(out detail);
            DocumentPlaceholderCollection<DetailDocument> toSerialize = master.DetailPlaceHolders;
            string myString = toSerialize.SerializeToString();
            Assert.IsNotNullOrEmpty(myString);
        }
    }
}