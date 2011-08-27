using System;
using System.Collections.Generic;
using System.Linq;
using GeniusCode.RavenDb.DataAccess;
using GeniusCode.RavenDb.Migrations;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Json.Linq;

namespace GeniusCode.RavenDb.Tests
{
    [TestFixture]
    public class MigrationDocument_Tests
    {
        private IDocumentStore _store = null;
        private IRavenRepository _repository = null;

        private RavenJObject GetStalePersonAsRavenJObject()
        {
            const string json = @"{
              ""Name"": ""Doe, John"",
              ""Age"": 15,
              ""@metadata"": {
                ""Raven-Entity-Name"": ""People"",
                ""Raven-Clr-Type"": ""GeniusCode.RavenDb.Tests.MigrationDocument_Tests+Person, GeniusCode.RavenDb.Tests"",
                ""Last-Modified"": ""2011-08-22T17:45:14.7600000-07:00"",
                ""Non-Authoritive-Information"": false,
                ""@id"": ""people/19457"",
                ""Temp-Index-Score"": 0.845849335193634,
                ""@etag"": ""00000000-0000-0400-0000-00000000005d""
              }
            }";
            return json.DeserializeToRavenJObject();
        }

        private readonly List<Person> _persons = new List<Person>();

        [TestFixtureSetUp]
        public void Init()
        {
            //DOCUMENT SESSION:
            var mockSession = new Mock<IDocumentSession>();

            //COMMANDS:
            var mockCommands = new Mock<IDatabaseCommands>();
            mockCommands.Setup(a => a.Batch(It.IsAny<IEnumerable<ICommandData>>())).Returns(new BatchResult[] { });

            // get query result
            var queryResult = new QueryResult();
            queryResult.Results.Add(GetStalePersonAsRavenJObject());
            mockCommands.Setup(a => a.Query(It.IsAny<string>(), It.IsAny<IndexQuery>(), It.IsAny<string[]>()))
                .Returns(queryResult)
                .Callback(() => queryResult.Results.Clear());

            // DOCUMENT STORE:
            var mockStore = new Mock<IDocumentStore>();

            mockStore.Setup(a => a.DatabaseCommands).Returns(mockCommands.Object);
            mockStore.Setup(a => a.OpenSession()).Returns(mockSession.Object);

            var mockrepository = new Mock<IRavenRepository>();
            mockrepository.Setup(a => a.All<Person>()).Returns(_persons);

            _store = mockStore.Object;
            _repository = mockrepository.Object;

            //_store = new DocumentStore
            //             {
            //                 Url = "http://localhost:8080"
            //             }.Initialize();

        }

        [TestFixtureTearDown]
        public void Cleanup()
        {
            //var session = _store.OpenSession();

            //var all = session.Query<Person>().GetAll();

            //all.ForEach(session.Delete);
            //session.SaveChanges();
        }

        [Test]
        public void Should_migrate_using_static_types()
        {

            Action<Person, Person_v1> updater = (p, p1) =>
            {
                var nameSegments = p1.Name.Split(',');
                p.LastName = nameSegments[0].Trim();
                p.FirstName = nameSegments[1].Trim();

                // shortcut: put the person object in the list
                _persons.Add(p);
            };

            var m = MigrationAction<Person, Person_v1>.AsMerge(updater);

            CommitMigration(m, _store);
            AssertMigrationsSuccessful();
        }

        private void AssertMigrationsSuccessful()
        {
            //var session = store.OpenSession();

            //var items = session.Query<Person>().Customize(a => a.WaitForNonStaleResultsAsOfNow())
            //    .GetAll();

            //var myItem = items.Where(a => a.LastName == "Doe").Single();

            var myItem = _repository.All<Person>().Single();

            Assert.AreEqual("Doe, John", myItem.FullName);
            Assert.AreEqual(15, myItem.Age);
        }

        private static void CommitMigration(IMigrationAction migrationAction, IDocumentStore store)
        {
            var host = new MigrationHost(store);
            host.Actions.Add(migrationAction);
            host.PerformActions();
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
