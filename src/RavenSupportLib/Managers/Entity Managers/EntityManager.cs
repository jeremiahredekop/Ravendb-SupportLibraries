using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    /// <summary>
    /// Represents a shortcut to AbstractEntityManager, passing in TEntity for both Sub & Super types
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TMd"></typeparam>
    /// <typeparam name="TSessionInfo"></typeparam>
    public abstract class EntityManager<TEntity, TMd, TSessionInfo> : AbstractEntityManager<TEntity, TMd, TSessionInfo, TEntity>,
        IEntityManager<TSessionInfo, TEntity, TMd>
        where TMd : class, ILinqMetaData, new()
        where TEntity : EntityBase2, IEntity2, new()
        where TSessionInfo : class
    {
    }
}