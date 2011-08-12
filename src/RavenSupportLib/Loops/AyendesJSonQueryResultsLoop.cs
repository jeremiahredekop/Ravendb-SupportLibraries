using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Json.Linq;
using System.Text.RegularExpressions;

namespace RavenSupportLib
{
    internal class AyendesJSonQueryResultsLoop : AyendesLoop<RavenJObject>
    {

        public AyendesJSonQueryResultsLoop(IDocumentStore documentStore, string indexName, string queryContents)
            : this(documentStore, indexName, queryContents, 128)
        {
        }
        public AyendesJSonQueryResultsLoop(IDocumentStore documentStore, string indexName, string queryContents, int querySize)
        {
            _QuerySize = querySize;
            _QueryContents = queryContents;
            _IndexName = indexName;
            _DocumentStore = documentStore;
        }

        private readonly IDocumentStore _DocumentStore;
        private readonly string _IndexName;
        private readonly string _QueryContents;
        private readonly int _QuerySize;

        protected override List<RavenJObject> PerformQuery(int queryStartPosition)
        {
            var q = _DocumentStore.DatabaseCommands.Query(_IndexName, new IndexQuery
            {
                Query = _QueryContents,
                PageSize = _QuerySize,
                Start = queryStartPosition
            }, null);

            return q.Results;
        }

        public List<ICommandData> GetCommandToModifyJSONItems(Func<RavenJObject, RavenJObject> toApply)
        {
            var cmds = new List<ICommandData>();

            var items = GetAll();

            items.ForEach(ri =>
                {
                    var itemToSave = toApply(ri);
                    if (itemToSave != null)
                    {
                        var putData = itemToSave.ToPutCommandData();
                        cmds.Add(putData);
                    }
                });

            return cmds;
        }
    }
}
