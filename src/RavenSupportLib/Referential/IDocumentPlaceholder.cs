using Newtonsoft.Json;

namespace GeniusCode.RavenDb.Referential
{
    public interface IDocumentPointerData
    {
        string Name { get; set; }
    }

    public interface IDocumentPlaceholder
    {
        string DocKey { get; }

        [JsonIgnore]
        int DocId { get; set; }

        object Data { get; set; }
    }
}