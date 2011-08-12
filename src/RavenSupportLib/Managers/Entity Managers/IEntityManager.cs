using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    // We need this for MEF Export purposes
    public interface IEntityManager
    {
        EntityBase2 CreateEntityInstance();
        void PersistEntity(EntityBase2 entity);
        void DeleteEntity(EntityBase2 entity);
    }

    // We need this for locating Entity Managers when TEntity is just EntityBase2 (e.g., from PersistContainer)
    public interface IEntityManager<TSessionInfo, TMetadata> : IDataManager<TSessionInfo, TMetadata>, IEntityManager
        where TSessionInfo : class
        where TMetadata : class, ILinqMetaData, new()
    {
    }

    // We need this because you might have a multiple-inheritance with abstract inheriting from abstract... where entity does not have a constructor
    public interface IEntityManagerCore<TSessionInfo, TEntity, TMetadata> : IEntityManager<TSessionInfo, TMetadata>
        where TSessionInfo : class
        where TEntity : EntityBase2, IEntity2
        where TMetadata : class, ILinqMetaData, new()
    {
        DataSource2<TEntity> DataSource { get; }
        TEntity CreateEntityInstance();
        void PersistEntity(TEntity entity);
        void DeleteEntity(TEntity entity);
    }

    // Caps the entity inheritance tree.  Requires parameterless constructor for TEntity
    public interface IEntityManager<TSessionInfo, TEntity, TMetadata> : IEntityManagerCore<TSessionInfo, TEntity, TMetadata>
        where TSessionInfo : class
        where TEntity : EntityBase2, IEntity2, new()
        where TMetadata : class, ILinqMetaData, new()
    {
    }


}
