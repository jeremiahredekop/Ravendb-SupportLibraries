namespace GeniusCode.RavenDb.Referential
{
    public interface IDocumentPlaceholderCollection
    {
        void AddIfNew(int targetId, IDocumentPointerData targetData);
    }
}