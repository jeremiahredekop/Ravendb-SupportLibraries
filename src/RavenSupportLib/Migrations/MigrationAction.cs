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
        private IMigrationAction _migrationAction;


        /// <summary>
        /// Returns a migration action that represents merging data.  The func will recieve a previous instance, 
        /// and must build a current instance to persist.
        /// </summary>
        /// <param name="toPerform">To perform.</param>
        /// <returns></returns>
        public static IMigrationAction AsReplace(Func<TPrevious, TCurrent> toPerform)
        {

            Func<RavenJObject, RavenJObject> updater = a =>
            {
                var previous = a.DeserializeToObject<TPrevious>();
                var current = toPerform(previous);

                var rjo = current.SerializeToRavenJObject(a.Value<RavenJObject>("@metadata"));
                return rjo;
            };

            var ma = new MigrationAction<TCurrent, TPrevious>
            {
                _migrationAction = MigrationAction.CreateForType<TCurrent>(updater)
            };

            return ma;
        }

        /// <summary>
        /// Returns a migration action that represents merging data.  The action will recieve a current and previous instance.
        /// The current instance will already have data contained.
        /// </summary>
        /// <param name="toPerform">To perform.</param>
        /// <returns></returns>
        public static IMigrationAction AsMerge(Action<TCurrent, TPrevious> toPerform)
        {

            Func<RavenJObject, RavenJObject> updater = a =>
            {
                var current = a.DeserializeToObject<TCurrent>();
                var previous = a.DeserializeToObject<TPrevious>();

                toPerform(current, previous);

                var rjo = current.SerializeToRavenJObject(a.Value<RavenJObject>("@metadata"));
                return rjo;
            };


            var ma = new MigrationAction<TCurrent, TPrevious>
            {
                _migrationAction = MigrationAction.CreateForType<TCurrent>(updater)
            };

            return ma;
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