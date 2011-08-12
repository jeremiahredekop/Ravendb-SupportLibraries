using System.Collections.Generic;
using GeniusCode.Framework.Extensions;
using GeniusCode.Framework.Support.Refection;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{

    /// <summary>
    /// Represents an abstract entity's manager.  Does not require constructor for entity subtype.
    /// </summary>
    /// <typeparam name="TSubtype"></typeparam>
    /// <typeparam name="TMd"></typeparam>
    /// <typeparam name="TSessionInfo"></typeparam>
    /// <typeparam name="TSuperType"></typeparam>
    public abstract class AbstractEntityManagerBase<TSubtype, TMd, TSessionInfo, TSuperType> : DataManager<TMd, TSessionInfo>,
            IEntityManagerCore<TSessionInfo, TSubtype, TMd>
        where TMd : class, ILinqMetaData, new()
        where TSuperType : EntityBase2, IEntity2
        where TSessionInfo : class
        where TSubtype : EntityBase2, TSuperType, IEntity2
    {
        private GeniusCode.Framework.Support.Objects.LazySource<DataSource2<TSubtype>> DataSourceHolder;

        public AbstractEntityManagerBase()
        {
            DataSourceHolder = new Support.Objects.LazySource<DataSource2<TSubtype>>(() => GetDefaultDataSource(Scope.Metadata));
        }

        public IEntityManagerCore<TSessionInfo, TSubtype, TMd> AsIEntityManager()
        {
            return this;
        }

        #region Protected Members

        protected DataSource2<TSubtype> DataSource
        {
            get
            {
                return DataSourceHolder;
            }
        }

        private void AddEntityForSaveToUoW(TSubtype entity, bool recursive, bool refetch)
        {
            Scope.UnitOfWork.AddForSave(entity, null, refetch, recursive);
        }

        private void AddEntityForDeleteToUoW(TSubtype toDelete)
        {
            Scope.UnitOfWork.AddForDelete(toDelete);
        }

        protected void PersistEntities(IEnumerable<TSubtype> entityCollection)
        {
            entityCollection.ForAll(o =>
            {
                PersistEntity(o);
            });
        }

        protected void PersistEntity(TSubtype entity)
        {
            //TODO:  What should these values be?
            this.AddEntityForSaveToUoW(entity: entity, recursive: true, refetch: true);
            OnEntityPersisted(entity);
        }

        protected void DeleteEntity(TSubtype entity)
        {
            this.AddEntityForDeleteToUoW(entity);
            OnEntityDeleted(entity);
        }

        #endregion

        #region Abstract Members
        protected abstract TSubtype CreateNewEntityInstance();
        #endregion


        #region Virtual Members


        protected virtual void OnEntityPersisted(TSubtype entity)
        {
        }

        protected virtual void OnEntityDeleted(TSubtype entity)
        {
        }

        protected virtual void OnEntityInstanceCreated(TSubtype entity)
        {
        }

        protected virtual DataSource2<TSubtype> GetDefaultDataSource(TMd md)
        {
            bool success;
            return (DataSource2<TSubtype>)ReflectionHelper.TryGetPropertyValue(typeof(TSubtype).Name.Replace("Entity", ""), md, null, out success);
        }

        #endregion


        #region Implementations

        void IEntityManagerCore<TSessionInfo, TSubtype, TMd>.PersistEntity(TSubtype entity)
        {
            PersistEntity(entity);
        }

        void IEntityManagerCore<TSessionInfo, TSubtype, TMd>.DeleteEntity(TSubtype entity)
        {
            DeleteEntity(entity);
        }

        DataSource2<TSubtype> IEntityManagerCore<TSessionInfo, TSubtype, TMd>.DataSource
        {
            get { return this.DataSource; }
        }

        TSubtype IEntityManagerCore<TSessionInfo, TSubtype, TMd>.CreateEntityInstance()
        {
            return CreateNewEntityInstance();
        }

        EntityBase2 IEntityManager.CreateEntityInstance()
        {
            return this.CreateNewEntityInstance();
        }
        void IEntityManager.PersistEntity(EntityBase2 entity)
        {
            // Requires same entity type
            this.PersistEntity((TSubtype)entity);
        }
        void IEntityManager.DeleteEntity(EntityBase2 entity)
        {
            // Requires same entity type
            this.AddEntityForDeleteToUoW((TSubtype)entity);
        }
        #endregion


    }
}
