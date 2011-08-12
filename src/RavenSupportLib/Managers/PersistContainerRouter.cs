using System;
using GeniusCode.Framework.Support.Data;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    public interface IPersistContainerRouter<T, TSession, TMetadata>
        where T : class
        where TSession : class
        where TMetadata : class, ILinqMetaData, new()
    {
        /// <summary>
        /// Routes a PersistContainer through Entity Managers and adds to Scope's UnitOfWork
        /// </summary>
        /// <param name="work"></param>
        /// <param name="adapter"></param>
        /// <returns></returns>
        void Route(PersistContainer<T> work, DataScope<TSession, TMetadata> scope);
    }

    /// <summary>
    /// Routes a PersistContainer through EntityManagers & adds to Scope's UnitOfWork
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TSession"></typeparam>
    /// <typeparam name="TMetaData"></typeparam>
    public class PersistContainerRouter<T, TSession, TMetadata> : IPersistContainerRouter<T, TSession, TMetadata>
        where T : class
        where TSession : class
        where TMetadata : class, ILinqMetaData, new()
    {
        EntityManagerSource<TSession, TMetadata> _source;
        Func<T, EntityBase2> _entityTransform;

        public PersistContainerRouter(EntityManagerSource<TSession, TMetadata> source, Func<T, EntityBase2> entityTransform)
        {
            _source = source;
            _entityTransform = entityTransform;
        }

        protected PersistContainerRouter(EntityManagerSource<TSession, TMetadata> source)
        {
            _source = source;
        }

        public void Route(PersistContainer<T> work, DataScope<TSession, TMetadata> scope)
        {
            work.ToCreate.ForEach(dto =>
                {
                    AddForSave(dto, scope);
                });

            work.ToUpdate.ForEach(dto =>
                {
                    AddForSave(dto, scope);
                });

            work.ToDelete.ForEach(dto =>
                {
                    AddForDelete(dto, scope);
                });
        }

        private void AddForSave(T dto, DataScope<TSession, TMetadata> scope)
        {
            var entity = GetLLBLGenEntity(dto);
            var entityManager = GetEntityManager(entity, scope);

            entityManager.PersistEntity(entity);
        }

        private void AddForDelete(T dto, DataScope<TSession, TMetadata> scope)
        {
            var entity = GetLLBLGenEntity(dto);
            var entityManager = GetEntityManager(entity, scope);

            entityManager.DeleteEntity(entity);
        }


        #region Helpers

        protected virtual IEntityManager<TSession, TMetadata> GetEntityManager(EntityBase2 entity, DataScope<TSession, TMetadata> scope)
        {
            return _source.GetManagerForEntity(entity, scope);
        }

        /// <summary>
        /// Converts a T WorkItem to an LLBLGen Entity
        /// </summary>
        /// <param name="workItem"></param>
        /// <returns></returns>
        protected virtual EntityBase2 GetLLBLGenEntity(T workItem)
        {
            return _entityTransform.Invoke(workItem);
        }

        #endregion
    }
}

