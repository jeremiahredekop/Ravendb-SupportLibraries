using SD.LLBLGen.Pro.LinqSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    public class DataManager<TSessionInfo> : DataManagerLink<TSessionInfo>
        where TSessionInfo : class
    {
        protected internal DataScope<TSessionInfo> Scope
        {
            get { return _Scope; }
            set { _Scope = value; }
        }
        protected internal EntityManagerSource<TSessionInfo> Source
        {
            get { return _Source; }
            set { _Source = value; }
        }
    }
}