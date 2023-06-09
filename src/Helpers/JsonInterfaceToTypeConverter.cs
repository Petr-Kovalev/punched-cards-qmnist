using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal sealed class JsonInterfaceToTypeConverter : JsonConverter<object>
    {
        private static readonly IReadOnlyDictionary<Type, Type> Mapping = new Dictionary<Type, Type>()
        {
            { typeof(IBitVector), typeof(BitVector) },
            { typeof(IExpert), typeof(Expert) }
        };

        public override bool CanConvert(Type typeToConvert)
        {
            return Mapping.TryGetValue(typeToConvert, out _);
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Mapping.TryGetValue(typeToConvert, out var returnType) ? System.Text.Json.JsonSerializer.Deserialize(ref reader, returnType, options) : null;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            System.Text.Json.JsonSerializer.Serialize(writer, value, options);
        }
    }
}