using Raven.Client;

namespace GeniusCode.Framework.LLBLGen.Managers
{
    public class DataScope<TSessionInfo>
        where TSessionInfo : class
    {
        public IDocumentStore Adapter { get; internal set; }
        public IDocumentSession UnitOfWork { get; internal set; }
        public TSessionInfo SessionInfo { get; internal set; }
    }
}
