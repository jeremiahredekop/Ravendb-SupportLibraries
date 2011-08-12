using System;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{

    /// <summary>
    /// A placeholder class that allows for a simple propagation of the session type variable for data managers, factories, and domain services
    /// No additional logic is added
    /// </summary>
    /// <typeparam name="TSession">The type of the session.</typeparam>
    public abstract class DALSupport<TSession, TMetaData>
        where TSession : class
        where TMetaData : class, ILinqMetaData, new()
    {


        public abstract class LogicHarness : TransactionHarness<TSession, TMetaData>
        {
            public LogicHarness(IAdapterService adapterService, Func<TSession> sessionInfoFunc, EntityManagerSource<TSession, TMetaData> source)
                : base(adapterService, sessionInfoFunc, source)
            {

            }
        }

        public class EntityManagerSource : EntityManagerSource<TSession, TMetaData>
        {
        }

        public class DataManager : DataManager<TMetaData, TSession>
        {
        }

        public class EntityManager<TEntity> : EntityManager<TEntity, TMetaData, TSession>
            where TEntity : EntityBase2, IEntity2, new()
        {
        }

        public abstract class AbstractEntityManager<TSubEntity, TSuperEntity> : AbstractEntityManager<TSubEntity, TMetaData, TSession, TSuperEntity>
            where TSuperEntity : EntityBase2, IEntity2
            where TSubEntity : EntityBase2, TSuperEntity, IEntity2, new()
        {
        }

        public class PersistContainerRouter<T> : PersistContainerRouter<T, TSession, TMetaData>
            where T : class
        {
            public PersistContainerRouter(EntityManagerSource<TSession, TMetaData> source, Func<T, EntityBase2> entityTransform)
                : base(source, entityTransform)
            {
            }
        }


    }

}
