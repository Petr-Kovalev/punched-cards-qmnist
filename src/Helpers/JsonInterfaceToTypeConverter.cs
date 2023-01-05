using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Runtime;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
	internal sealed class JsonInterfaceToTypeConverter : JsonConverter
    {
        private static readonly IDictionary<string, Type> Mapping = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        static JsonInterfaceToTypeConverter()
        {
            Map<IBitVector, BitVector>();
            Map<IExpert, Expert>();
        }

        public override bool CanConvert(Type objectType)
        {
            return Mapping.TryGetValue(objectType.Name, out _);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            Newtonsoft.Json.JsonSerializer serializer)
        {
            return Mapping.TryGetValue(objectType.Name, out var type) ? serializer.Deserialize(reader, type) : null;
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        private static void Map<TI, T>() where T : class, TI
        {
            Mapping.Add(typeof(TI).Name, typeof(T));
        }
    }
}