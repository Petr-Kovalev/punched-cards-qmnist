using PunchedCards.BitVectors;
using System.Text.Json.Serialization;

namespace PunchedCards.Helpers
{
    [JsonSerializable(typeof(BitVector))]
    [JsonSerializable(typeof(IReadOnlyList<ValueTuple<IBitVector, IBitVector>>))]
    [JsonSerializable(typeof(Expert))]
    [JsonSerializable(typeof(IReadOnlyDictionary<string, IExpert>))]
    internal partial class SourceGenerationContext : JsonSerializerContext;
}