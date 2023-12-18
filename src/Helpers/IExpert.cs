using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal interface IExpert
    {
        public IReadOnlyDictionary<IBitVector, double> CalculateMatchingScores(IBitVector bitVector);

        public bool FineTune(IBitVector bitVector, IBitVector label);
    }
}