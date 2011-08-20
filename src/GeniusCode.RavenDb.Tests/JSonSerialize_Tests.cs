using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeniusCode.RavenDb.Tests.MasterDetailDocuments;
using NUnit.Framework;

namespace GeniusCode.RavenDb.Tests
{
    [TestFixture]
    public class JSonSerialize_Tests
    {

        [Test]
        public void ShouldSerialize()
        {

            var master = new MasterDocument() {Name = "Frank"};
            var masterString = master.SerializeToString();
            var master2 = masterString.DeserializeFromString<MasterDocument>();
            Assert.AreEqual("Frank",master2.Name);
        }

        [Test]
        public void ShouldSerializeCollection()
        {
            DetailDocument detail;
            var master = Helpers.BuildSimpleMasterAndDetail(out detail);
            var toSerialize = master.DetailPlaceHolders;
            var myString = toSerialize.SerializeToString();
            Assert.IsNotNullOrEmpty(myString);
        }



        [Test]
        public void ShouldDeserializeCollection()
        {
            const string myString = "{\"Items\":[{\"DocKey\":\"DetailDocuments/25\",\"Name\":\"Cheetos\"}]}";
            var obj = myString.DeserializeFromString<DocumentPlaceholderCollection<DetailDocument>>();
            Assert.IsNotNull(obj);
        }
        


    }
}
