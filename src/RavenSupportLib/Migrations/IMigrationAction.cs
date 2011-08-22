using System;
using Raven.Json.Linq;

namespace GeniusCode.RavenDb.Migrations
{
    public interface IMigrationAction
    {
        string IndexName { get; }
        string QueryContents { get; }
        Func<RavenJObject, RavenJObject> GetReplacement { get; }
    }
}