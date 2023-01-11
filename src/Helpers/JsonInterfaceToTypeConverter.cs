using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal sealed class JsonInterfaceToTypeConverter : JsonConverter<object>
    {
        private static readonly IDictionary<Type, Type> Mapping = new Dictionary<Type, Type>();

        static JsonInterfaceToTypeConverter()
        {
            Map<IBitVector, BitVector>();
            Map<IExpert, Expert>();
        }

        public override bool CanConvert(Type objectType)
        {
            return Mapping.TryGetValue(objectType, out _);
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Mapping.TryGetValue(typeToConvert, out var type) ? System.Text.Json.JsonSerializer.Deserialize(ref reader, type, options) : null;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            System.Text.Json.JsonSerializer.Serialize(writer, value, options);
        }

        private static void Map<TI, T>() where T : class, TI
        {
            Mapping.Add(typeof(TI), typeof(T));
        }
    }
}