using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace PunchedCards.Helpers
{
	internal sealed class JsonSerializer
	{
        private const int DefaultBufferSize = 80 * 1024; // 80K is the value from modern stream extension method CopyTo implementation

        private static readonly Encoding Utf8WithoutBom = new UTF8Encoding(false);

        private readonly Newtonsoft.Json.JsonSerializer _jsonSerializer;

        private JsonSerializer()
		{
            _jsonSerializer = Newtonsoft.Json.JsonSerializer.Create(new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters =
                {
                    new JsonInterfaceToTypeConverter()
                }
            });
        }

		internal static JsonSerializer Instance { get; } = new JsonSerializer();

        internal void Serialize(object value, StreamWriter streamWriter)
        {
            using (var jsonTextWriter = new JsonTextWriter(streamWriter)
            {
                CloseOutput = false,
                AutoCompleteOnClose = false
            })
            {
                _jsonSerializer.Serialize(jsonTextWriter, value);
            }
        }

        internal T Deserialize<T>(StreamReader streamReader)
        {
            using (var jsonTextReader = new JsonTextReader(streamReader)
            {
                CloseInput = false
            })
            {
                return _jsonSerializer.Deserialize<T>(jsonTextReader);
            }
        }
    }
}

