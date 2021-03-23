namespace PunchedCards.Helpers
{
    internal interface IAdjacencyMatrix
    {
        public uint Size { get; }

        public uint this[int i, int j] { get; }

        public ulong HalfSum { get; }
    }
}