using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    public class DataManagerLink<TSessionInfo> : IDataManager<TSessionInfo>
        where TSessionInfo : class
    {
        internal DataScope<TSessionInfo> _Scope;
        internal EntityManagerSource<TSessionInfo> _Source;
        EntityManagerSource<TSessionInfo> IDataManager<TSessionInfo>.Source
        {
            get
            {
                return this._Source;
            }
            set
            {
                _Source = value;
            }
        }
        DataScope<TSessionInfo> IDataManager<TSessionInfo>.Scope
        {
            get
            {
                return this._Scope;
            }
            set
            {
                this._Scope = value;
            }
        }
        protected IEntityManager<TSessionInfo, TEntity> GetManagerForEntity<TEntity>()
                where TEntity : new()
        {
            return _Source.GetManagerForEntity<TEntity>(this._Scope);
        }
        protected TDataManager GetDataManager<TDataManager>()
                where TDataManager : class, IDataManager<TSessionInfo>, new()
        {
            return _Source.GetDataManager<TDataManager>(this._Scope);
        }
    }
}
