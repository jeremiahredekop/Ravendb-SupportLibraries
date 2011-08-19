using System;
using System.Collections.Generic;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;

namespace GeniusCode.RavenDb.Loops
{
    internal class AyendesJSonQueryResultsLoop : AyendesLoop<RavenJObject>
    {
        private readonly IDocumentStore _documentStore;
        private readonly string _indexName;
        private readonly string _queryContents;
        private readonly int _querySize;

        public AyendesJSonQueryResultsLoop(IDocumentStore documentStore, string indexName, string queryContents)
            : this(documentStore, indexName, queryContents, 128)
        {
        }

        public AyendesJSonQueryResultsLoop(IDocumentStore documentStore, string indexName, string queryContents,
                                           int querySize)
        {
            _querySize = querySize;
            _queryContents = queryContents;
            _indexName = indexName;
            _documentStore = documentStore;
        }

        protected override List<RavenJObject> PerformQuery(int queryStartPosition)
        {
            QueryResult q = _documentStore.DatabaseCommands.Query(_indexName, new IndexQuery
                                                                                  {
                                                                                      Query = _queryContents,
                                                                                      PageSize = _querySize,
                                                                                      Start = queryStartPosition
                                                                                  }, null);

            return q.Results;
        }

        public List<ICommandData> GetCommandToModifyJSONItems(Func<RavenJObject, RavenJObject> toApply)
        {
            var cmds = new List<ICommandData>();

            List<RavenJObject> items = GetAll();

            items.ForEach(ri =>
                              {
                                  RavenJObject itemToSave = toApply(ri);
                                  if (itemToSave != null)
                                  {
                                      PutCommandData putData = itemToSave.ToPutCommandData();
                                      cmds.Add(putData);
                                  }
                              });

            return cmds;
        }
    }
}