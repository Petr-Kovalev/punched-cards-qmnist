using Microsoft.Extensions.DependencyInjection;
using PunchedCards.BitVectors;
using PunchedCards.Helpers;

namespace PunchedCards
{
    internal sealed class RandomPuncher : IPuncher<string, IBitVector, IBitVector>
    {
        private static readonly Random Random = new(42);

        private readonly uint _bitCount;
        private readonly IBitVectorFactory _bitVectorFactory;

        private uint _lastCount;
        private uint[][] _map;

        internal RandomPuncher(uint bitCount)
        {
            _bitCount = bitCount;
            _bitVectorFactory = DependencyInjection.ServiceProvider.GetService<IBitVectorFactory>();
        }

        public IPunchedCard<string, IBitVector> Punch(string key, IBitVector input)
        {
            return new PunchedCard<string, IBitVector>(key, Punch(input, _map[int.Parse(key)]));
        }

        public IEnumerable<string> GetKeys(uint count)
        {
            if (_lastCount != count)
            {
                _lastCount = count;
                ReinitializeMap(count);
            }

            return Enumerable.Range(0, _map.Length).Select(index => index.ToString());
        }

        private IBitVector Punch(IBitVector bitVector, IReadOnlyCollection<uint> indices)
        {
            return _bitVectorFactory.Create(PunchActiveBitIndices(bitVector, indices), (uint) indices.Count);
        }

        private static IEnumerable<uint> PunchActiveBitIndices(IBitVector bitVector, IEnumerable<uint> indices)
        {
            var currentBitIndex = 0U;
            foreach (var index in indices)
            {
                if (bitVector.IsActive(index))
                {
                    yield return currentBitIndex;
                }

                currentBitIndex++;
            }
        }

        private void ReinitializeMap(uint count)
        {
            var usedIndicesHashSet = new HashSet<uint>();
            var rowsCount = count / _bitCount;
            _map = new uint[rowsCount][];

            for (var i = 0; i < rowsCount; i++)
            {
                var indices = new uint[_bitCount];
                for (var j = 0; j < _bitCount; j++)
                {
                    uint index;
                    do
                    {
                        index = (uint) Random.Next((int) count);
                    } while (!usedIndicesHashSet.Add(index));

                    indices[j] = index;
                }

                _map[i] = indices;
            }
        }
    }
}