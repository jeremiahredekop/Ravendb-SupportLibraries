using System.Diagnostics;
using System.Dynamic;
using GeniusCode.RavenDb.Data;
using GeniusCode.RavenDb.Referential;
using GeniusCode.RavenDb.Tests.MasterDetailDocuments;
using NUnit.Framework;
using System;

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
            var e2 = Helpers.SerializeCopyWithJSON<ExpandoObject>(e);
            Assert.AreEqual("World", e2.Hello);
        }

        [Test]
        public void Should_serialize_bag_into_concrete_type()
        {
            var bag = new Bag();

            bag.BagData.Hello = "World";
            bag.BagData.Number = 1;
            bag.BagData.BoolValue = false;

            var asString = bag.SerializeToString();

            var toContainer = asString.DeserializeFromString<Container>();

            Assert.AreEqual("World", toContainer.BackingExpando.Hello);
            Assert.AreEqual(1, toContainer.BackingExpando.Number);
            Assert.AreEqual(false, toContainer.BackingExpando.BoolValue);

        }

        [Test]
        public void Should_seraialize_concrete_type_into_bag()
        {
            var concrete = new Container() {BackingExpando = new Stuff()};

            concrete.BackingExpando.Hello = "World";
            concrete.BackingExpando.Number = 1;
            concrete.BackingExpando.BoolValue = false;

            var asString = concrete.SerializeToString();
            var toBag = asString.DeserializeFromString<Bag>();

            Assert.AreEqual("World", toBag.BagData.Hello);
            Assert.AreEqual(1, toBag.BagData.Number);
            Assert.AreEqual(false, toBag.BagData.BoolValue);
        }


        public class Container
        {
            public Stuff BackingExpando { get; set; }
        }

        public class Stuff
        {
            public string Hello { get; set; }
            public int Number { get; set; }
            public bool BoolValue { get; set; }
        }

    }
}
