using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    /// <summary>
    /// Repesents the top of the entity inheritance tree. Requires default constructor for entity subtype.
    /// </summary>
    /// <typeparam name="TSubtype"></typeparam>
    /// <typeparam name="TMd"></typeparam>
    /// <typeparam name="TSessionInfo"></typeparam>
    /// <typeparam name="TSuperType"></typeparam>
    public abstract class AbstractEntityManager<TSubtype, TMd, TSessionInfo, TSuperType>
        : AbstractEntityManagerBase<TSubtype, TMd, TSessionInfo, TSuperType>, IEntityManager<TSessionInfo, TSubtype, TMd>
        where TMd : class, ILinqMetaData, new()
        where TSuperType : EntityBase2, IEntity2
        where TSessionInfo : class
        where TSubtype : EntityBase2, TSuperType, IEntity2, new()
    {
        protected override sealed TSubtype CreateNewEntityInstance()
        {
            TSubtype output = new TSubtype();
            OnEntityInstanceCreated(output);

            return output;
        }
    }
}
