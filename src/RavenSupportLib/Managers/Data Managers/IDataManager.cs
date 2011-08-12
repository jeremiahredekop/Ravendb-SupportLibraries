using System;
using SD.LLBLGen.Pro.LinqSupportClasses;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    public interface IDataManager<TScope, TSessionInfo>
        where TSessionInfo : class
    {
        DataScope<TSessionInfo> Scope { get; set; }
        EntityManagerSource<TSessionInfo> Source { get; set; }
    }
}
