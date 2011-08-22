using System;
using Raven.Client.Util;
using Raven.Json.Linq;

namespace GeniusCode.RavenDb.Migrations
{
    public class MigrationAction : IMigrationAction
    {

        public static MigrationAction CreateForType<T>(Action<RavenJObject> modifier)
        {
            Func<RavenJObject, RavenJObject> getReplacement = a =>
                                                                  {
                                                                      modifier(a);
                                                                      return a;
                                                                  };

            return CreateForType<T>(getReplacement);
        }

        public static MigrationAction CreateForType<T>(Func<RavenJObject, RavenJObject> getReplacement)
        {
            var ma = new MigrationAction("Raven/DocumentsByEntityName", "Tag:" + Inflector.Pluralize(typeof(T).Name),
                                         getReplacement);
            return ma;
        }

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

    public class MigrationAction<TCurrent, TPrevious> : IMigrationAction
    {
        private readonly Action<TCurrent, TPrevious> _toPerform;
        private readonly IMigrationAction _migrationAction;

        public MigrationAction(Action<TCurrent, TPrevious> toPerform)
        {
            _toPerform = toPerform;

            Func<RavenJObject, RavenJObject> updater = a =>
            {
                var current = a.DeserializeToObject<TCurrent>();
                var previous = a.DeserializeToObject<TPrevious>();

                _toPerform(current, previous);

                var rjo = current.SerializeToRavenJObject(a.Value<RavenJObject>("@metadata"));
                return rjo;
            };

            _migrationAction = MigrationAction.CreateForType<TCurrent>(updater);
        }

        #region Implementation of IMigrationAction

        public string IndexName
        {
            get { return _migrationAction.IndexName; }
        }

        public string QueryContents
        {
            get { return _migrationAction.QueryContents; }
        }

        public Func<RavenJObject, RavenJObject> GetReplacement
        {
            get
            {
                return _migrationAction.GetReplacement;
            }
        }

        #endregion
    }

}