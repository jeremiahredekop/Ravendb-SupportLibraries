using System.IO;
using Newtonsoft.Json;
using Raven.Json.Linq;

namespace System
{
    public static class RavenJObjectExtensions
    {
        public static string ToJsonText(this RavenJObject input)
        {
            var writer = new StringWriter();
            JsonWriter jwriter = new JsonTextWriter(writer);
            input.WriteTo(jwriter);
            return writer.ToString();
        }

        public static T DeserializeToObject<T>(this RavenJObject input)
        {
            var itemText = input.ToJsonText();
            var item = itemText.DeserializeFromString<T>();

            return item;
        }

        internal static RavenJObject SerializeToRavenJObject(this object input, RavenJObject metadata)
        {
            var obj = RavenJObject.FromObject(input);
            obj["@metadata"] = metadata;
            return obj;
        }

    }
}