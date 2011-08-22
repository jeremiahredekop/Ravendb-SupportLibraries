using System;
using System.Linq;
using GeniusCode.RavenDb.DataAccess;
using GeniusCode.RavenDb.Migrations;
using Newtonsoft.Json;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;
using Raven.Json.Linq;

namespace GeniusCode.RavenDb.Tests
{
    [TestFixture]
    public class MigrationDocument_Tests
    {
        private IDocumentStore _store = null;

        [TestFixtureSetUp]
        public void Init()
        {
            _store = new DocumentStore()
                         {
                             Url = "http://localhost:8080"
                         }.Initialize();
        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            var session = _store.OpenSession();

            var all = session.Query<Person>().GetAll();

            all.ForEach(session.Delete);
            session.SaveChanges();
        }

        [Test]
        public void Should_migrate_using_static_types()
        {

            SeedOldData(_store);

            //NOTE: this class will wrap all the JObject stuff, and query operations
            Action<Person, Person_v1> updater = (p, p1) =>
            {
                var nameSegments = p1.Name.Split(',');
                p.LastName = nameSegments[0].Trim();
                p.FirstName = nameSegments[1].Trim();
            };

            var m = new MigrationAction<Person, Person_v1>(updater);

            CommitMigration(m, _store);
            AssertMigrationsSuccessful(_store);
        }

        private void AssertMigrationsSuccessful(IDocumentStore store)
        {
            var session = store.OpenSession();

            var items = session.Query<Person>().Customize(a => a.WaitForNonStaleResultsAsOfNow())
                .GetAll();

            var myItem = items.Where(a => a.LastName == "Doe").Single();

            Assert.AreEqual("Doe, John", myItem.FullName);
            Assert.AreEqual(15, myItem.Age);
        }

        private void CommitMigration(MigrationAction<Person, Person_v1> migrationAction, IDocumentStore store)
        {
            var host = new MigrationHost(store);
            host.Actions.Add(migrationAction);
            host.PerformActions();
        }

        private static void SeedOldData(IDocumentStore store)
        {
            var session = store.OpenSession();

            var p1 = new Person
                         {
                             Age = 15,
                             FirstName = "John",
                             LastName = "Doe"
                         };

            session.Store(p1);
            session.SaveChanges();


            var asString = String.Empty;
            var ma = MigrationAction.CreateForType<Person>(a =>
                                                         {
                                                             var firstName = a.Value<string>("FirstName");
                                                             a.Remove("FirstName");
                                                             var lastName = a.Value<string>("LastName");
                                                             a.Remove("LastName");

                                                             a["Name"] = string.Format("{0}, {1}", lastName,
                                                                                           firstName);

                                                             asString = a.ToJsonText();
                                                         });

            

            var host = new MigrationHost(store);
            host.Actions.Add(ma);
            host.PerformActions();

            Assert.IsTrue(asString.Contains("Name\":\"Doe, John\""), "Test is not ready.");
        }


        /// <summary>
        /// This is a person class back from v1
        /// </summary>
        public class Person_v1
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }

        /// <summary>
        /// This is a person class from the current version
        /// </summary>
        public class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

            [JsonIgnore]
            public string FullName { get { return string.Format("{0}, {1}", LastName, FirstName); } }
            public int Age { get; set; }
        }



    }
}
