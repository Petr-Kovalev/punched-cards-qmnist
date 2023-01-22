using System.IO;
using System.Text.Json;

namespace PunchedCards.Helpers
{
    internal static class JsonSerializer
	{
        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
                {
                    new JsonInterfaceToTypeConverter()
                }
        };

        internal static void Serialize<T>(T value, Stream stream)
        {
            System.Text.Json.JsonSerializer.Serialize(stream, value, JsonSerializerOptions);
        }

        internal static T Deserialize<T>(Stream stream)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(stream, JsonSerializerOptions);
        }
    }
}