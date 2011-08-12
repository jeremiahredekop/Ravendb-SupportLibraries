using System;
using GeniusCode.Framework.Composition;
using GeniusCode.Framework.Support.Refection;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using GeniusCode.Framework.Caching;
using GeniusCode.Framework.Support.Mocking;

namespace GeniusCode.Framework.LLBLGen.Managers
{

    public class EntityManagerSource<TSessionInfo> : ObjectSource<IEntityManager<TSessionInfo, TMetadata>, IEntityManager<TSessionInfo, TMetadata>, EntityManagerLocateArgs>
        where TSessionInfo : class
        where TMetadata : class, ILinqMetaData, new()
    {


        #region Overrides

        protected virtual void OnAcquiredEntityManager(IEntityManager<TSessionInfo, TMetadata> manager, DataScope<TSessionInfo, TMetadata> scope)
        {
        }


        protected virtual void OnAcquiredDataManager(IDataManager<TSessionInfo, TMetadata> manager, DataScope<TSessionInfo, TMetadata> scope)
        {
            manager.Scope = scope;
            manager.Source = this;
        }

        protected sealed override ICacheAdapter<string, IEntityManager<TSessionInfo, TMetadata>> Get_CacheAdapter()
        {
            return null;
        }

        protected sealed override IMockProvider Get_MockingProvider()
        {
            return null;
        }

        #endregion

        #region Public Members

        public DataScope<TSessionInfo, TMetadata> GetDataScope(IDataAccessAdapter adapter, TSessionInfo sessionInfo, FunctionMappingStore functionMappings)
        {
            var uow = new UnitOfWork2();
            return GetDataScope(adapter, sessionInfo, functionMappings, uow);
        }

        public DataScope<TSessionInfo, TMetadata> GetDataScope(IDataAccessAdapter adapter, TSessionInfo sessionInfo, FunctionMappingStore functionMappings, UnitOfWork2 uow)
        {
            TMetadata md = new TMetadata();
            ReflectionHelper.SetPropertyValue(md, "AdapterToUse", adapter);

            if (functionMappings != null)
                ReflectionHelper.SetPropertyValue(md, "CustomFunctionMappings", functionMappings);

            var output = new DataScope<TSessionInfo, TMetadata>()
            {
                Adapter = adapter,
                Metadata = md,
                UnitOfWork = uow,
                SessionInfo = sessionInfo
            };

            return output;
        }

        public DataScope<TSessionInfo, TMetadata> GetDataScope(IDataAccessAdapter adapter, TSessionInfo sessionInfo, UnitOfWork2 uow)
        {
            FunctionMappingStore functionMappings = null;
            return GetDataScope(adapter, sessionInfo, functionMappings, uow);
        }

        public DataScope<TSessionInfo, TMetadata> GetDataScope(IDataAccessAdapter adapter, TSessionInfo sessionInfo)
        {
            FunctionMappingStore functionMappings = null;
            return GetDataScope(adapter, sessionInfo, functionMappings);
        }


        public IEntityManager<TSessionInfo, TMetadata> GetManagerForEntity(EntityBase2 entity, DataScope<TSessionInfo, TMetadata> scope)
        {
            var type = entity.GetType();
            var mi = new RelayReflectedMethod<EntityManagerSource<TSessionInfo, TMetadata>, IEntityManager<TSessionInfo, TMetadata>, DataScope<TSessionInfo, TMetadata>>(
                "GetManagerForEntity", this, System.Reflection.BindingFlags.Default, new Type[] { type }, null, null);
            return mi.Invoke(scope);
        }


        public IEntityManager<TSessionInfo, TEntity, TMetadata> GetManagerForEntity<TEntity>(DataScope<TSessionInfo, TMetadata> scope)
            where TEntity : EntityBase2, new()
        {
            var args = base.BuildDefaultArgs();
            args.EntityType = typeof(TEntity);
            var output = base.CreateInstance<IEntityManager<TSessionInfo, TEntity, TMetadata>>(args);
            OnAcquiredDataManager(output, scope);
            OnAcquiredEntityManager(output, scope);
            return output;
        }

        public TEntityManager GetEntityManager<TEntityManager>(DataScope<TSessionInfo, TMetadata> scope)
            where TEntityManager : class, IEntityManager<TSessionInfo, TMetadata>, new()
        {
            var args = base.BuildDefaultArgs();
            var output = base.CreateInstance<TEntityManager>(args);
            OnAcquiredDataManager(output, scope);
            OnAcquiredEntityManager(output, scope);
            return output;
        }

        public TDataManager GetDataManager<TDataManager>(DataScope<TSessionInfo, TMetadata> scope)
    where TDataManager : class, IDataManager<TSessionInfo, TMetadata>, new()
        {
            var output = new TDataManager();
            OnAcquiredDataManager(output, scope);
            return output;
        }

        #endregion

    }

}
