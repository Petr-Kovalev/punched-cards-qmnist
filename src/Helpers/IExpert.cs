using System.Collections.Generic;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal interface IExpert
    {
        public double CalculateMatchingScore(IBitVector bitVector, IBitVector label);

        public IReadOnlyDictionary<IBitVector, double> CalculateMatchingScores(IBitVector bitVector);
    }
}