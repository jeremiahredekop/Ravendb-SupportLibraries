using System;
using System.Collections.Generic;
using Raven.Client.Linq;
namespace RavenSupportLib
{
    public interface IRavenRepository
    {
        void Add<T>(T item);
        IEnumerable<T> All<T>();
        void Delete<T>(T item);
        T SingleOrDefault<T>(Func<T, bool> predicate);
        IRavenQueryable<T> Query<T>();
        IRavenQueryable<T> Query<T, TIndexCreator>() where TIndexCreator : Raven.Client.Indexes.AbstractIndexCreationTask, new();
        T GetById<T>(int id);
        T GetByKey<T>(string key);
        T[] GetByKeys<T>(IEnumerable<string> keys);
    }

}
