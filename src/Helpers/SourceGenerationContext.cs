using PunchedCards.BitVectors;
using System.Text.Json.Serialization;

namespace PunchedCards.Helpers
{
    [JsonSerializable(typeof(IBitVector))]
    [JsonSerializable(typeof(BitVector))]
    [JsonSerializable(typeof(IReadOnlyList<ValueTuple<IBitVector, IBitVector>>))]
    [JsonSerializable(typeof(List<ValueTuple<IBitVector, IBitVector>>))]
    [JsonSerializable(typeof(IExpert))]
    [JsonSerializable(typeof(Expert))]
    [JsonSerializable(typeof(IReadOnlyDictionary<string, IExpert>))]
    internal partial class SourceGenerationContext : JsonSerializerContext;
}