using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client;
using Raven.Client.Linq;

namespace RavenSupportLib
{
    public interface IDocument
    {
        string Id { get; set; }
    }

    public sealed class RavenRepository : IRavenRepository
    {
        #region Constuctors
        public RavenRepository(IDocumentSession session, bool liveQueries)
        {
            _session = session;
            _LiveQueries = liveQueries;
        }
        #endregion

        #region Helpers


        private IRavenQueryable<T> GetQuery<T, TIndexCreator>()
             where TIndexCreator : Raven.Client.Indexes.AbstractIndexCreationTask, new()
        {

            if (_LiveQueries)
                return _session.Query<T, TIndexCreator>().Customize(a => a.WaitForNonStaleResultsAsOfNow());

            return _session.Query<T, TIndexCreator>();
        }
        private IRavenQueryable<T> GetQuery<T>()
        {
            if (_LiveQueries)
                return _session.Query<T>().Customize(a => a.WaitForNonStaleResultsAsOfNow());

            return _session.Query<T>();
        }
        #endregion


        #region Assets

        private readonly bool _LiveQueries;
        private readonly IDocumentSession _session;
        #endregion

        public T SingleOrDefault<T>(Func<T, bool> predicate)
        {
            return GetQuery<T>().Where(predicate).SingleOrDefault(predicate);
        }

        public IEnumerable<T> All<T>()
        {
            return GetQuery<T>().GetAll();
        }

        public void Add<T>(T item)
        {
            _session.Store(item);
        }

        public void Delete<T>(T item)
        {
            _session.Delete(item);
        }

        public void Save()
        {
            _session.SaveChanges();
        }

        public IRavenQueryable<T> Query<T>()
        {
            return GetQuery<T>();
        }

        public T GetById<T>(int id)
        {

            return _session.Load<T>(id);
        }


        public T GetByKey<T>(string key)
        {
            return _session.Load<T>(key);
        }


        public IRavenQueryable<T> Query<T, TIndexCreator>()
             where TIndexCreator : Raven.Client.Indexes.AbstractIndexCreationTask, new()
        {
            return GetQuery<T, TIndexCreator>();
        }

        public T[] GetByKeys<T>(IEnumerable<string> ids)
        {
            return _session.Load<T>(ids: ids);
        }

    }
}
