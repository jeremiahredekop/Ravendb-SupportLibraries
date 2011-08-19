using System;
using Raven.Json.Linq;

namespace GeniusCode.RavenDb.Migrations
{
    public class MigrationAction
    {
        public MigrationAction(string indexName, string queryContents, Func<RavenJObject, RavenJObject> getReplacement)
        {
            GetReplacement = getReplacement;
            QueryContents = queryContents;
            IndexName = indexName;
        }

        public string IndexName { get; private set; }
        public string QueryContents { get; private set; }
        public Func<RavenJObject, RavenJObject> GetReplacement { get; private set; }
    }
}