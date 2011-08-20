using Newtonsoft.Json;

namespace GeniusCode.RavenDb
{
    public interface IDocumentPlaceholder
    {
        string DocKey { get; }

        [JsonIgnore]
        int DocId { get; set; }

        string Name { get; set; }
    }
}