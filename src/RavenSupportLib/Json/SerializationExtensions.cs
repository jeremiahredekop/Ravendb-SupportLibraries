using System.IO;
using Newtonsoft.Json;

namespace System
{
    public static class SerializationExtensions
    {
        public static string SerializeToString<T>(this T input)
        {
            var writer = new StringWriter();
            var jsonSerializer = new JsonSerializer();
            jsonSerializer.Serialize(writer, input);
            return writer.ToString();
        }

        public static T DeserializeFromString<T>(this string input)
        {
            var reader = new StringReader(input);
            var jreader = new JsonTextReader(reader);
            var jsonSerializer = new JsonSerializer();
            var output = jsonSerializer.Deserialize<T>(jreader);
            return output;
        }
    }
}