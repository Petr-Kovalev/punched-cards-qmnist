using System.Collections.Generic;

namespace PunchedCards.Helpers
{
    internal interface IAdjacencyMatrix
    {
        public uint Size { get; }

        public ulong MaxSpanningTree { get; }

        public ulong CalculateMaxSpanningTreeMatchingScore(IEnumerable<uint> activeBitIndices);
    }
}